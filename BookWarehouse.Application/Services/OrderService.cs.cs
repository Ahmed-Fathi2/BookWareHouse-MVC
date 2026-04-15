using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Domain.Common.Enums;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Mapster;
using Stripe.Checkout;

namespace BookWarehouse.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork ,
        ICartService cartService,
        IStripePaymentService stripePaymentService,
        IKashierPaymentService kashierPaymentService) : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICartService _cartService = cartService;
        private readonly IStripePaymentService _stripePaymentService = stripePaymentService;
        private readonly IKashierPaymentService _kashierPaymentService = kashierPaymentService;

        public async Task<Result<string>> PlaceOrderAsync(string origin, CheckoutVM checkoutVM)
        {
            // 1. Get Cart
            var cartItemsResult = await _cartService.GetAllUserCartProducts(checkoutVM.ApplicationUserId);

            if (!cartItemsResult.IsSuccess || !cartItemsResult.Value.Any())
                return Result.Failure<string>(new Error("Cart is empty", "Cart.Empty"));

            var cartItems = cartItemsResult.Value;

            Order order;

            // 2. DB Transaction (ONLY DB)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                order = await GetOrCreateOrderAsync(checkoutVM, cartItems);

                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<string>(new Error(ex.Message, "Order.Failed"));
            }

            // 3. External Call (Stripe) — OUTSIDE transaction
            //var sessionResult = await _stripePaymentService
            //    .CreateCheckoutSessionAsync(origin, cartItems, order.Id);


            var sessionResult = await _kashierPaymentService.InitiatePaymentAsync(origin, order.Id);

            if (!sessionResult.IsSuccess)
                return Result.Failure<string>(sessionResult.Error);

            return Result.Success(sessionResult.Value);
        }


        private async Task<Order> GetOrCreateOrderAsync(CheckoutVM checkoutVM, IEnumerable<CartDetailsVM> cartItems)
        {
            var cartSignature = GenerateCartSignature(cartItems);

            var existingOrders = await _unitOfWork.OrderRepository
                .GetAllAsync(o =>
                    o.ApplicationUserId == checkoutVM.ApplicationUserId && (o.OrderStatus == OrderStatus.Pending),
                    tracked:true);

            var order = existingOrders
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefault();

            //1- If no existing order, create new
            if (order == null)
                return await CreateNewOrder(checkoutVM, cartItems, cartSignature);

            //2- If order exists and cart signature matches, reuse it
            if (order.CartSignature == cartSignature)
            {
                order.OrderStatus = OrderStatus.Pending;
                order.PaymentStatus = PaymentStatus.Pending;
                order.SessionId = null;
                order.PaymentIntentId = null;
            
                await _unitOfWork.SaveChangesAsync();
                return order;
            }

            //3- If order exists but cart has changed, cancel old order and create new one
            order.OrderStatus = OrderStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();
            return await CreateNewOrder(checkoutVM, cartItems, cartSignature);
        }

        private async Task<Order> CreateNewOrder(CheckoutVM checkoutVM, IEnumerable<CartDetailsVM> cartItems,string signature)
        {
            var newOrder = checkoutVM.Adapt<Order>();
            newOrder.CartSignature= signature;

            _unitOfWork.OrderRepository.Add(newOrder);

            await _unitOfWork.SaveChangesAsync();


            var orderDetailsList = cartItems.Select(item => new OrderDetails
            {
                OrderId = newOrder.Id,
                ProductId = item.ProductId,
                Quantity = item.Count,
                Price = item.FinalPrice / item.Count

            }).ToList();


            _unitOfWork.OrderDetailsRepository.AddRange(orderDetailsList);

            await _unitOfWork.SaveChangesAsync();

            return newOrder;
        }

        private string GenerateCartSignature(IEnumerable<CartDetailsVM> cartItems)
        {
            return string.Join("|", cartItems
                .OrderBy(x => x.ProductId)
                .Select(x => $"{x.ProductId}-{x.Count}"));
        }
    }
}



/*
 # 💳 Payment Flow Documentation (Stripe Integration)

## 🎯 Overview

This document describes the complete payment flow using Stripe, including:

* Order creation & reuse
* Checkout session handling
* Webhook processing
* All edge cases (cancel, failure, cart changes)

---

# 🧠 Core Concept

The system is built on:

* **Pending Order Lifecycle**
* **Cart Signature Comparison**
* **Webhook-driven confirmation**

---

# 🧾 Order Lifecycle

| Status    | Description                        |
| --------- | ---------------------------------- |
| Pending   | Order created, waiting for payment |
| Approved  | Payment successful                 |
| Cancelled | Cart changed → old order invalid   |
| Failed    | Payment failed                     |

---

# 🔁 Main Flow

## 1. User clicks Checkout

→ `PlaceOrderAsync`

### Steps:

1. Get user cart
2. Generate `CartSignature`
3. Call `GetOrCreateOrderAsync`
4. Create Stripe Checkout Session
5. Redirect user to Stripe

---

## 2. Get or Create Order Logic

### 🟢 Case 1: No existing order

→ Create new order (Pending)

---

### 🟢 Case 2: Same cart

```csharp
order.CartSignature == cartSignature
```

→ Reuse existing order
→ Reset:

* PaymentStatus = Pending
* SessionId = null
* PaymentIntentId = null

---

### 🔴 Case 3: Cart changed

→ Cancel old order
→ Create new order

---

# 💳 Stripe Checkout

## Session Created with:

* Success URL → `/Cart/OrderConfirmation?id={orderId}`
* Cancel URL → `/Cart/Index`
* Metadata:

  * `order_id`

---

# 🔔 Webhook Handling

## Endpoint:

```
POST /stripe/webhook
```

---

## 🟢 Event: checkout.session.completed

### Steps:

1. Get session
2. Read `order_id` from metadata
3. Get order from DB

### Idempotency:

```csharp
if (order.PaymentStatus == Paid) return;
```

### Update:

* PaymentStatus = Paid
* OrderStatus = Approved
* Save PaymentIntentId
* Save PaymentDate

---

## 🔴 Event: payment_intent.payment_failed

### Steps:

1. Get PaymentIntent
2. Read `order_id` from metadata
3. Get order

### Update:

* PaymentStatus = Failed
* OrderStatus = Pending (or Cancelled based on logic)

---

# ❌ Cancel / Back / Close Tab

## Important Behavior:

Stripe **does NOT send webhook** for:

* Cancel
* Back
* Closing tab

---

## System Handling:

* Order remains **Pending**
* No DB update happens
* User is redirected to Cart

---

# 🔁 Retry Payment

## Scenario:

User returns without changing cart

→ Same order reused
→ New session created

---

# 🔄 Cart Modification

## Scenario:

User modifies cart after cancel

→ Old order → Cancelled
→ New order → Pending

---

# 🧠 Cart Signature

Used to detect changes:

```csharp
ProductId + Quantity
```

Example:

```
1-2|5-1|9-3
```

---

# ⚠️ Edge Cases Handled

| Scenario          | Handling              |
| ----------------- | --------------------- |
| Payment success   | Webhook updates order |
| Payment failed    | Webhook updates order |
| User cancel       | No change (Pending)   |
| User closes tab   | No change (Pending)   |
| Retry same cart   | Reuse order           |
| Modify cart       | Cancel + new order    |
| Duplicate webhook | Ignored (idempotency) |

---

# 🔐 Security

* Webhook signature verification
* No trust on frontend redirect
* All final states confirmed via webhook

---

# 🚀 Final Flow Summary

```
User Checkout
    ↓
Get/Create Order
    ↓
Create Stripe Session
    ↓
Redirect to Stripe
    ↓
    ├── Success
    │       ↓
    │   Webhook → Approve Order
    │       ↓
    │   Redirect → Confirmation Page
    │
    └── Cancel / Close
            ↓
        Return to Cart
            ↓
        Order remains Pending
```

---

# ✅ Conclusion

This system ensures:

* No duplicate orders
* Safe payment confirmation
* Flexible retry mechanism
* Accurate order tracking

---

 */
using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Order;
using BookWarehouse.Domain.Common.Enums;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Mapster;
using Stripe.Checkout;

namespace BookWarehouse.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork,
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
            var sessionResult = await _stripePaymentService
                .CreateCheckoutSessionAsync(origin, cartItems, order.Id);


            //var sessionResult = await _kashierPaymentService.InitiatePaymentAsync(origin, order.Id);

            //if (!sessionResult.IsSuccess)
            //    return Result.Failure<string>(sessionResult.Error);

            return Result.Success(sessionResult.Value);
        }


        private async Task<Order> GetOrCreateOrderAsync(CheckoutVM checkoutVM, IEnumerable<CartDetailsVM> cartItems)
        {
            var cartSignature = GenerateCartSignature(cartItems);

            var existingOrders = await _unitOfWork.OrderRepository
                .GetAllAsync(o =>
                    o.ApplicationUserId == checkoutVM.ApplicationUserId && (o.OrderStatus == OrderStatus.Pending),
                    tracked: true);

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

        private async Task<Order> CreateNewOrder(CheckoutVM checkoutVM, IEnumerable<CartDetailsVM> cartItems, string signature)
        {
            var newOrder = checkoutVM.Adapt<Order>();
            newOrder.CartSignature = signature;

            _unitOfWork.OrderRepository.Add(newOrder);

            await _unitOfWork.SaveChangesAsync();


            var orderDetailsList = cartItems.Select(item => new OrderDetails
            {
                OrderId = newOrder.Id,
                ProductId = item.ProductId,
                Quantity = item.Count,
                Price = item.FinalPrice / item.Count // Price of a single item

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

        public async Task<Result<IEnumerable<OrderReadVM>>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();

            var orderReadVMs = orders.Adapt<IEnumerable<OrderReadVM>>();

            return Result.Success(orderReadVMs);

        }

        public async Task<Result<OrderDetailsVM>> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetOrderDetails(orderId);
            if (order == null)
                return Result.Failure<OrderDetailsVM>(new Error("Order not found", "Order.NotFound"));

            var result = order.Adapt<OrderDetailsVM>();

            return Result.Success(result);
        }
        public async Task<Result> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result.Failure(new Error("Order not found", "Order.NotFound"));

   
                if (status == OrderStatus.Shipped)
                    order.ShippingDate = DateTime.Now;

                
                order.OrderStatus = status;

                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
 
        }

        public async Task<Result> UpdateOrderDetailsAsync(int orderId, string carrier, string trackingNumber)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result.Failure(new Error("Order not found", "Order.NotFound"));

            order.Carrier = carrier;
            order.TrackingNumber = trackingNumber;
   

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
    }
}



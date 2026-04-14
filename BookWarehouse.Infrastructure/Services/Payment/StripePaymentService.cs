using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Domain.Common.Enums;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace BookWarehouse.Infrastructure.Services.Payment
{
    public class StripePaymentService(ILogger<StripePaymentService> logger, IUnitOfWork unitOfWork) : IStripePaymentService
    {
        private readonly ILogger<StripePaymentService> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> CreateCheckoutSessionAsync(string origin, IEnumerable<CartDetailsVM> cartDetailsVMs, Order order)
        {

            var metadata = new Dictionary<string, string>
            {
                { "order_id", order.Id.ToString() }
            };

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{origin}/Cart/OrderConfirmation?orderId={order.Id}",
                CancelUrl = $"{origin}/Cart/Index",
                //CancelUrl = $"{origin}/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",

                Metadata = metadata,  //  on Session

                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = metadata  //  on PaymentIntent 
                }
            };

            foreach (var item in cartDetailsVMs)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)((item.FinalPrice / item.Count) * 100), // $20.50 => 2050 // Price of a single item
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }


            var customerOrder = await _unitOfWork.OrderRepository.GetByIdAsync(order.Id);
            if (customerOrder == null)
                throw new Exception($"Order with ID {order.Id} not found.");

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            customerOrder.SessionId = session.Id;

            await _unitOfWork.SaveChangesAsync();


            return Result.Success(session.Url);


        }

        /*
        public async Task<Result<string>> CreateNewSessionForOrderAsync(int orderId, string origin)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                return Result.Failure<string>(new Error("OrderNotFound", "Order not found."));

            var orderDetails = await _unitOfWork.OrderDetailsRepository.GetAllAsync(
                od => od.OrderId == orderId,
                new System.Linq.Expressions.Expression<Func<OrderDetails, object>>[] { od => od.Product }
            );

            var cartDetailsVMs = orderDetails.Select(od => new CartDetailsVM
            {
                Title = od.Product.Title,
                FinalPrice = od.Price * od.Quantity,
                Count = od.Quantity
            }).ToList();

            return await CreateCheckoutSessionAsync(origin, cartDetailsVMs, order);
        }
        */

        public async Task<Result> HandleStripeWebhookAsync(string json, string signature)
        {

            //var webhookSecret = _config["Stripe:WebhookSecret"];
            var webhookSecret = "whsec_0325ee89f2fb4ec57e243b68fe0e4919201bee74ba59739756190d32e70d6d27";

            Event stripeEvent;

            try
            {
                // 4. Verify the event came from Stripe (not a fake request)
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    webhookSecret
                );
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed");
                //return BadRequest("Invalid signature");
                return Result.Failure(new Error("Invalid signature", "StripeWebhook"));
            }

            // 5. Handle the event type
            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                    await HandleCheckoutSessionCompleted(stripeEvent);
                    break;

                case EventTypes.PaymentIntentPaymentFailed:
                    await HandlePaymentFailed(stripeEvent);
                    break;

                default:
                    _logger.LogInformation("Unhandled Stripe event: {Type}", stripeEvent.Type);
                    break;
            }

            // 6. Always return 200 to Stripe — otherwise it retries
            return Result.Success();
        }

        private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return;

            // ✅ Read from Session metadata
            if (!session.Metadata.TryGetValue("order_id", out var orderIdStr))
            {
                _logger.LogWarning("order_id missing from session metadata");
                return;
            }

            if (!int.TryParse(orderIdStr, out var orderId)) return;

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) return;

            // Idempotency check
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                _logger.LogInformation("Order {Id} already marked as Paid. Skipping duplicate webhook event.", order.Id);
                return;
            }

            if (session.PaymentStatus == "paid")
            {
                order.SessionId = session.Id;
                order.PaymentIntentId = session.PaymentIntentId;
                order.PaymentDate = DateTime.UtcNow;
                order.PaymentStatus = PaymentStatus.Paid;
                order.OrderStatus = OrderStatus.Approved;

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Order {Id} approved", orderId);
            }
        }

        private async Task HandlePaymentFailed(Event stripeEvent)
        {
            // If Payment Failed , Stripe Sends PaymentIntent object in the event data, not Session
            //PAymentIntent is Object in Stripe that represents a single payment attempt. It tracks the lifecycle of the payment, including any failures and their reasons.
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            // ✅ Read from PaymentIntent metadata
            if (!paymentIntent.Metadata.TryGetValue("order_id", out var orderIdStr))
            {
                _logger.LogWarning("order_id missing from PaymentIntent metadata");
                return;
            }

            if (!int.TryParse(orderIdStr, out var orderId)) return;
            // ✅ Idempotency (avoid duplicate processing)
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) return;

            if (order.PaymentStatus == PaymentStatus.Failed)
            {
                _logger.LogInformation("Order {Id} already marked as failed, skipping", orderId);
                return;
            }

            order.PaymentIntentId = paymentIntent.Id;
            order.PaymentStatus = PaymentStatus.Failed;
            order.OrderStatus = OrderStatus.Pending;

            await _unitOfWork.SaveChangesAsync();
            // ✅ Logging
            _logger.LogWarning(
                "Payment failed for Order {Id}, Reason: {Reason}",
                orderId,
                paymentIntent.LastPaymentError?.Message
            );
        }
    }
}



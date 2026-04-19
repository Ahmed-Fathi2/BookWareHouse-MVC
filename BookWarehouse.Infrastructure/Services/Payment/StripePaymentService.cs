using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.Comman.Settings;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Payment;
using BookWarehouse.Domain.Common.Enums;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace BookWarehouse.Infrastructure.Services.Payment
{
    public class StripePaymentService(ILogger<StripePaymentService> logger, IUnitOfWork unitOfWork,IOptions<StripeSetting> options) : IPaymentService
    {
        private readonly ILogger<StripePaymentService> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly StripeSetting _options = options.Value;

        public async Task<Result<string>> CreateCheckoutSessionAsync(string origin, int orderId, IEnumerable<CartDetailsVM>? cartDetailsVMs)
        {

            var metadata = new Dictionary<string, string>
            {
                { "order_id", orderId.ToString() }
            };

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{origin}/Cart/OrderConfirmation?orderId={orderId}",
                CancelUrl = $"{origin}/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",

                Metadata = metadata,  //  on Session

                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = metadata  //  on PaymentIntent 
                }
            };

            foreach (var item in cartDetailsVMs!)
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


            var customerOrder = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (customerOrder == null)
                return Result.Failure<string>(new Error("OrderNotFound", $"Order with ID {orderId} not found."));

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            customerOrder.SessionId = session.Id;

            await _unitOfWork.SaveChangesAsync();

            return Result.Success(session.Url);


        }

        public async Task<Result<WebHookVM>> HandleWebhookAsync(string body, string signature)
        {

            var webhookSecret = _options.WebhookSecret;

            Event stripeEvent;

            try
            {

                stripeEvent = EventUtility.ConstructEvent(
                    body,
                    signature,
                    webhookSecret
                );
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed");
                return Result.Failure<WebHookVM>(new Error("InvalidSignature", "Stripe webhook signature verification failed"));
            }


            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var result = await HandleCheckoutSessionCompleted(stripeEvent);
                if (result != null)
                    return Result.Success(result);
            }
            else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                var result = await HandlePaymentFailed(stripeEvent);
                if (result != null)
                    return Result.Success(result);
            }

            _logger.LogInformation("Unhandled Stripe event type: {Type}", stripeEvent.Type);
            return Result.Failure<WebHookVM>(new Error("UnhandledEventType", $"No handler for Stripe event type {stripeEvent.Type}"));


        }

        private async Task<WebHookVM?> HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return null;

            if (!session.Metadata.TryGetValue("order_id", out var orderIdStr))
            {
                _logger.LogWarning("order_id missing from session metadata");
                return null;
            }

            if (!int.TryParse(orderIdStr, out var orderId)) return null;

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) return null;

            var webHookVM = new WebHookVM
            {
                Status = PaymentWebhookStatus.SUCCESS.ToString(), 
                TransactionId = session.PaymentIntentId,
                OrderId = orderId
            };
            return webHookVM;


          
        }

        private async Task<WebHookVM?> HandlePaymentFailed(Event stripeEvent)
        {
         
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return null; 
   
            if (!paymentIntent.Metadata.TryGetValue("order_id", out var orderIdStr))
            {
                _logger.LogWarning("order_id missing from PaymentIntent metadata");
                return null;
            }

            if (!int.TryParse(orderIdStr, out var orderId)) return null;
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) return null;

            var webHookVM = new WebHookVM
            {
                OrderId = orderId,
                TransactionId = paymentIntent.Id,
                Status = PaymentWebhookStatus.FAILURE.ToString()    
            };
            return webHookVM;

            ////  Idempotency check: If we already marked this order as failed, no need to do it again
            //if (order.PaymentStatus == PaymentStatus.Failed)
            //{
            //    _logger.LogInformation("Order {Id} already marked as failed, skipping", orderId);
            //    return;
            //}

            //order.PaymentIntentId = paymentIntent.Id;
            //order.PaymentStatus = PaymentStatus.Failed;
            //order.OrderStatus = OrderStatus.Pending;

            //await _unitOfWork.SaveChangesAsync();

            //_logger.LogWarning(
            //    "Payment failed for Order {Id}, Reason: {Reason}",
            //    orderId,
            //    paymentIntent.LastPaymentError?.Message
            //);
        }
    }
}



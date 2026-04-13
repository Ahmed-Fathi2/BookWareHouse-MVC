using Azure;
using Azure.Core;
using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Services.Payment
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly ILogger<StripePaymentService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public StripePaymentService(ILogger<StripePaymentService> logger , IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<string>> CreateCheckoutSessionAsync(string origin, IEnumerable<CartDetailsVM> cartDetailsVMs, Order order)
        {

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{origin}/Cart/OrderConfirmation?id={order.Id}",
                CancelUrl = $"{origin}/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
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


           var customerOrder= await _unitOfWork.OrderRepository.GetByIdAsync(order.Id);
            if (customerOrder == null)
                throw new Exception($"Order with ID {order.Id} not found.");

            var service = new SessionService();
            Session session = service.Create(options);

            customerOrder.SessionId= session.Id;

           await _unitOfWork.SaveChangesAsync();
      

            return Result.Success(session.Url);


        }

      

        //public async Task<Result> HandleStripeWebhookAsync(string json, string signature)
        //{


        //    // 3. Get your webhook secret from config

        //    //var webhookSecret = _config["Stripe:WebhookSecret"];
        //    var webhookSecret = "";

        //    Event stripeEvent;

        //    try
        //    {
        //        // 4. Verify the event came from Stripe (not a fake request)
        //        stripeEvent = EventUtility.ConstructEvent(
        //            json,
        //            signature,
        //            webhookSecret
        //        );
        //    }
        //    catch (StripeException ex)
        //    {
        //        _logger.LogError(ex, "Stripe webhook signature verification failed");
        //        //return BadRequest("Invalid signature");
        //        return Result.Failure(new Error("Invalid signature", "StripeWebhook"));
        //    }

        //    // 5. Handle the event type
        //    switch (stripeEvent.Type)
        //    {
        //        case Events.CheckoutSessionCompleted:
        //            await HandleCheckoutSessionCompleted(stripeEvent);
        //            break;

        //        case Events.PaymentIntentPaymentFailed:
        //            await HandlePaymentFailed(stripeEvent);
        //            break;

        //        default:
        //            _logger.LogInformation("Unhandled Stripe event: {Type}", stripeEvent.Type);
        //            break;
        //    }

        //    // 6. Always return 200 to Stripe — otherwise it retries
        //  return  Result.Success();
        //}

        //private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        //{
        //    var session = stripeEvent.Data.Object as Session;

        //    if (session == null) return;

        //    // Find the order by SessionId
        //    var orderHeader = _unitOfWork.OrderHeader
        //        .Get(o => o.SessionId == session.Id, includeProperties: "ApplicationUser");

        //    if (orderHeader == null)
        //    {
        //        _logger.LogWarning("Order not found for session {SessionId}", session.Id);
        //        return;
        //    }

        //    // Avoid processing the same event twice (idempotency)
        //    if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
        //    {
        //        _logger.LogInformation("Order {Id} already approved, skipping", orderHeader.Id);
        //        return;
        //    }

        //    if (session.PaymentStatus == "paid")
        //    {
        //        _unitOfWork.OrderHeader.UpdateStripePaymentID(
        //            orderHeader.Id, session.Id, session.PaymentIntentId);

        //        _unitOfWork.OrderHeader.UpdateStatus(
        //            orderHeader.Id, SD.StatusApproved, SD.PaymentStatusApproved);

        //        _unitOfWork.Save();

        //        _logger.LogInformation("Order {Id} approved via webhook", orderHeader.Id);
        //    }
        //}

        //private async Task HandlePaymentFailed(Event stripeEvent)
        //{
        //    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

        //    if (paymentIntent == null) return;

        //    var orderHeader = _unitOfWork.OrderHeader
        //        .Get(o => o.PaymentIntentId == paymentIntent.Id);

        //    if (orderHeader != null)
        //    {
        //        _unitOfWork.OrderHeader.UpdateStatus(
        //            orderHeader.Id, SD.StatusCancelled, SD.PaymentStatusRejected);

        //        _unitOfWork.Save();

        //        _logger.LogWarning("Payment failed for Order {Id}", orderHeader.Id);
        //    }
        //}
    }
    }

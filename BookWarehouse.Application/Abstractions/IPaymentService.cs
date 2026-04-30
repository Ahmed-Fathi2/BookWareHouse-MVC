using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Payment;
using BookWarehouse.Domain.Entities;
using Stripe.Checkout;

namespace BookWarehouse.Application.Abstractions
{
    public interface IPaymentService
    {
        //IPaymentService
        Task<Result<string>> CreateCheckoutSessionAsync(string origin, int orderId, IEnumerable<CartDetailsVM>? cartDetailsVMs);
        Task<Result<WebHookVM>> HandleWebhookAsync(string body, string signature);

        // Refund method (optional)
         Task<Result> RefundPaymentAsync(string merchantOrderId, string transactionId, decimal amount);
    }
}

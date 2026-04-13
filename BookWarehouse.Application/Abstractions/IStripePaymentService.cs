using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Domain.Entities;
using Stripe.Checkout;

namespace BookWarehouse.Application.Abstractions
{
    public interface IStripePaymentService
    {
        Task<Result<string>> CreateCheckoutSessionAsync(string origin,IEnumerable<CartDetailsVM> cartDetailsVMs , Order order);
        //Task<Result> HandleStripeWebhookAsync(string json, string signature);
    }
}

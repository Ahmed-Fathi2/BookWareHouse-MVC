using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using Stripe.Checkout;

namespace BookWarehouse.Application.Abstractions
{
    public interface IOrderService
    {
        Task<Result<string>> PlaceOrderAsync(string origin, CheckoutVM checkoutVM);

        //Task<Result> DeleteCanceledOrder(int orderId); 
    }
}

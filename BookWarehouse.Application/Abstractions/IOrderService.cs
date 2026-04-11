using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;

namespace BookWarehouse.Application.Abstractions
{
    public interface IOrderService
    {
        Task<Result> PlaceOrderAsync(CheckoutVM checkoutVM);
    }
}

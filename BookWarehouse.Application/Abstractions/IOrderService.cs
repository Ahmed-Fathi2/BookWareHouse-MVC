using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Order;
using BookWarehouse.Domain.Common.Enums;
using Stripe.Checkout;

namespace BookWarehouse.Application.Abstractions
{
    public interface IOrderService
    {
        Task<Result<string>> PlaceOrderAsync(string origin, CheckoutVM checkoutVM);


        Task<Result<IEnumerable<OrderReadVM>>> GetAllOrdersAsync();
        Task<Result<OrderDetailsVM>> GetOrderByIdAsync(int orderId);

        Task<Result> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<Result> UpdateOrderDetailsAsync(int orderId, string carrier, string trackingNumber);

        //Task<Result> DeleteCanceledOrder(int orderId); 
    }
}

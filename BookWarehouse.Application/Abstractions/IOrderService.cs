using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;
using BookWarehouse.Application.ViewModels.Order;
using BookWarehouse.Application.ViewModels.Payment;
using BookWarehouse.Domain.Common.Enums;
using BookWarehouse.Domain.Entities;
using Stripe.Checkout;

namespace BookWarehouse.Application.Abstractions
{
    public interface IOrderService
    {
        Task<Result<string>> PlaceOrderAsync(string origin, CheckoutVM checkoutVM);


        Task<Result<IEnumerable<OrderReadVM>>> GetAllOrdersAsync(OrderStatus? orderStatus);
        Task<Result<OrderDetailsVM>> GetOrderDeatilsByIdAsync(int orderId);

        Task<Result> CancelOrderAsync(int orderId);

        Task<Result> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<Result> UpdateOrderDetailsAsync(int orderId, string carrier, string trackingNumber);
        Task HandlePaymentResult(WebHookVM webHookVM);

      
    }
}

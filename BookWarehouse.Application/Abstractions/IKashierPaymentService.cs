using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Cart;

namespace BookWarehouse.Application.Abstractions
{
    public interface IKashierPaymentService
    {
        Task<Result<string>> InitiatePaymentAsync(string origin,int orderId);
    }
}

using BookWarehouse.Domain.Entities;
using System.Linq.Expressions;

namespace BookWarehouse.Domain.Repositories
{
    public interface IOrderRepository: IGenericRepository<Order,int>
    {
        Task<Order?> GetOrderDetails(int  orderId);
        Task<Order?> GetOrderById(int orderId); // Remove
        Task<IEnumerable<Order>> GetAllOrders(Expression<Func<Order, bool>>? filter = null);
    }
}

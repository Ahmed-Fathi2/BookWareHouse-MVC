using BookWarehouse.Domain.Entities;

namespace BookWarehouse.Domain.Repositories
{
    public interface IOrderRepository: IGenericRepository<Order,int>
    {
        Task<Order?> GetOrderDetails(int  orderId); 
    }
}

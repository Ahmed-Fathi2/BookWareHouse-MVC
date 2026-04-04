using BookWarehouse.Domain.Entities;

namespace BookWarehouse.Domain.Repositories
{
    public interface ICartRepository:IGenericRepository<Cart,Guid>
    {
    }
}

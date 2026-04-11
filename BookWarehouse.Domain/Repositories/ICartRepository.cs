using BookWarehouse.Domain.Entities;
using System.Linq.Expressions;

namespace BookWarehouse.Domain.Repositories
{
    public interface ICartRepository:IGenericRepository<Cart,int>
    {
        Task<Cart?> GetCartByFilter(Expression<Func<Cart, bool>> filter );
    }
}

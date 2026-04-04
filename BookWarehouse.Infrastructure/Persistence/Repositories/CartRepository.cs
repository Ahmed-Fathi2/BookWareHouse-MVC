using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class CartRepository:GenericRepository<Cart,Guid>, ICartRepository
    {
        public CartRepository(ApplicationDbContext dbContext) : base(dbContext) { }
 
    }
}

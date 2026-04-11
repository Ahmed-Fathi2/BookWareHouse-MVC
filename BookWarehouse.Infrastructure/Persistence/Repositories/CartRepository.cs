using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class CartRepository(ApplicationDbContext dbContext) : GenericRepository<Cart,int>(dbContext), ICartRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Cart?> GetCartByFilter(Expression<Func<Cart, bool>> filter)
        {
            var cart = await _dbContext.Carts.FirstOrDefaultAsync(filter);
            return cart;
        }
    }
}

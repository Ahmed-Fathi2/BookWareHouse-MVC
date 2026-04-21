using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class OrderRepository(ApplicationDbContext dbContext) : GenericRepository<Order,int>(dbContext), IOrderRepository
    {
        private readonly ApplicationDbContext dbContext = dbContext;

        public async Task<Order?> GetOrderDetails(int orderId)
        {
            var order = await _dbContext.Orders
                .Include(x => x.ApplicationUser)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            return order;
                
        }

        public async Task<Order?> GetOrderById(int orderId)
        {
            var order = await _dbContext.Orders
                            .AsTracking()
                            .FirstOrDefaultAsync(x => x.Id == orderId);

            return order;

        }

        public async Task<IEnumerable<Order>> GetAllOrders(Expression<Func<Order,bool>>?filter=null)
        {

            var query = _dbContext.Orders
                .Include(x => x.ApplicationUser)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }
    }
}

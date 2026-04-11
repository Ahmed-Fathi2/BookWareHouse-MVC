using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class OrderRepository(ApplicationDbContext dbContext) : GenericRepository<Order,int>(dbContext), IOrderRepository
    {
        private readonly ApplicationDbContext dbContext = dbContext;
    }
}

using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class OrderDetailsRepository(ApplicationDbContext dbContext) : GenericRepository<OrderDetails,int>(dbContext), IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
    }
}

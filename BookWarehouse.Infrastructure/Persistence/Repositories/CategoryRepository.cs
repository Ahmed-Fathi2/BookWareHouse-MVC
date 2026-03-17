using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository:GenericRepository<Category,Guid>,ICategoryRepository
    {

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }
      
    }
}

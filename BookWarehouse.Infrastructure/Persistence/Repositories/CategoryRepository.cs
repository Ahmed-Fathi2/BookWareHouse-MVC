using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

       
    }
}

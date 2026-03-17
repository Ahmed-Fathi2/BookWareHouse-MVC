using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public ICategoryRepository CategoryRepository { get; }

        public UnitOfWork(ApplicationDbContext dbContext , ICategoryRepository categoryRepository)
        {
            this.dbContext = dbContext;
            CategoryRepository = categoryRepository;
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}

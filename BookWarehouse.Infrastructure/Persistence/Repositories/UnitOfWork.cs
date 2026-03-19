using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public ICategoryRepository CategoryRepository { get; }

        public IProductRepository ProductRepository {  get; }

        public UnitOfWork(
            ApplicationDbContext dbContext ,
            ICategoryRepository categoryRepository ,
            IProductRepository productRepository
            )
        {
            this.dbContext = dbContext;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}

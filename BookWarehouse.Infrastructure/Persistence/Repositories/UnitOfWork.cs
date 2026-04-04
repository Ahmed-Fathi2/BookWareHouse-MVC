using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        public ICategoryRepository CategoryRepository { get; }

        public IProductRepository ProductRepository {  get; }

        public ICartRepository CartRepository{  get; }

        public UnitOfWork(
            ApplicationDbContext dbContext ,
            ICategoryRepository categoryRepository ,
            IProductRepository productRepository,
            ICartRepository cartRepository
            )
        {
            this.dbContext = dbContext;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
            CartRepository = cartRepository;
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}

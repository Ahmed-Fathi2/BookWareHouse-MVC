using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        private IDbContextTransaction? _transaction;
        public ICategoryRepository CategoryRepository { get; }

        public IProductRepository ProductRepository { get; }

        public ICartRepository CartRepository { get; }

        public IOrderRepository OrderRepository { get; }

        public IOrderDetailsRepository OrderDetailsRepository { get; }

        public IUserRepository UserRepository { get; }

        public UnitOfWork(
            ApplicationDbContext dbContext,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IOrderDetailsRepository orderDetailsRepository,
                IUserRepository userRepository
            )
        {
            this.dbContext = dbContext;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
            CartRepository = cartRepository;
            OrderRepository = orderRepository;
            OrderDetailsRepository = orderDetailsRepository;
            UserRepository = userRepository;
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            // to avoid nested transactions
            if (_transaction != null)
                return;

            _transaction = await dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await dbContext.SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}

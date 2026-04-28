using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Domain.Repositories
{
    public interface IUnitOfWork: IDisposable
    {
        public ICategoryRepository CategoryRepository { get;}
        public IProductRepository  ProductRepository  { get;}
        public ICartRepository CartRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderDetailsRepository OrderDetailsRepository { get; }

        public IUserRepository UserRepository { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();


        Task SaveChangesAsync();
    }
}

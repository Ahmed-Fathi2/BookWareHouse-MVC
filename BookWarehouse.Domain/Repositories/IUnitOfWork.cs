using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get;}
        public IProductRepository  ProductRepository  { get;}
        public ICartRepository CartRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderDetailsRepository OrderDetailsRepository { get; }

        Task SaveChangesAsync();
    }
}

using BookWarehouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Domain.Repositories
{
    public interface IProductRepository: IGenericRepository<Product, int> 
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync(
            Func<Product, bool>? filter = null,
            params Func<Product, object>[] includes);
    }
}

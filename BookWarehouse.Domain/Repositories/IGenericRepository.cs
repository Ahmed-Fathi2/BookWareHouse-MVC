using BookWarehouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Domain.Repositories
{
    public interface IGenericRepository<TEntity,TKey> where TEntity : class
    {

        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TKey id);

        void Add(TEntity category);

       void  Delete (TEntity category);

    }
}

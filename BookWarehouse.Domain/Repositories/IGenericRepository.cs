using BookWarehouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BookWarehouse.Domain.Repositories
{
    public interface IGenericRepository<TEntity,TKey> where TEntity : class
    {

        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity,bool>>? filter = null,
            Expression<Func<TEntity, object>>[]? includes = null,
            bool tracked = false);
        Task<TEntity?> GetByIdAsync(TKey id);

        void Add(TEntity entity );

        void AddRange(IEnumerable<TEntity> entities);

        void  Delete (TEntity entity);

        void DeleteAll(IEnumerable<TEntity> entities);

    }
}

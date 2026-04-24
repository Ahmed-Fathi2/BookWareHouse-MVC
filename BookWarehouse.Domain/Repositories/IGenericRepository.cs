using BookWarehouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BookWarehouse.Domain.Repositories
{
    public interface IGenericRepository<TEntity,TKey> where TEntity : class
    {

        Task<IQueryable<TEntity>> GetAllAsync(
                  Expression<Func<TEntity, bool>>? filter = null, // Filtering && Searching
                  Expression<Func<TEntity, object>>[]? includes = null, // Eager Loading
                  string? SortBy = null,
                  bool IsDescending = false,
                  bool tracked = false);

        Task<TEntity?> GetByIdAsync(TKey id);

        void Add(TEntity entity );

        void AddRange(IEnumerable<TEntity> entities);

        void  Delete (TEntity entity);

        void DeleteAll(IEnumerable<TEntity> entities);

    }
}

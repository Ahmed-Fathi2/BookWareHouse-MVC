using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey>(ApplicationDbContext dbContext) : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly ApplicationDbContext _dbContext = dbContext;


        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }


        public void Add(TEntity entity)
        {
            _dbContext.Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Remove(entity);
        }

        public async Task<IQueryable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null, 
            Expression<Func<TEntity, object>>[]? includes = null, 
            string? SortBy = null,
            bool IsDescending = false,
            bool tracked = false)

        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (!tracked)
                query = query.AsNoTracking();

            if (filter is not null)
                query = query.Where(filter);


            if (includes is not null && includes.Length > 0)
            {
                foreach (var inc in includes)
                {
                    query = query.Include(inc);
                }
            }

            if (!string.IsNullOrEmpty(SortBy))
            {
                var sortingDirection = IsDescending ? "DESC" : "ASC";

                query = query.OrderBy($"{SortBy} {sortingDirection}");
            }


            return query;

        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _dbContext.AddRange(entities);
        }

        public void DeleteAll(IEnumerable<TEntity> entities)
        {
            _dbContext.RemoveRange(entities);
        }
    }
}

using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey>(ApplicationDbContext dbContext) : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly ApplicationDbContext _dbContext = dbContext;

     
        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);  
        }

    
        public void Add(TEntity category)
        {
            _dbContext.Add(category);
        }

        public void Delete(TEntity category)
        {
            _dbContext.Remove(category);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>[]? includes = null,
            bool tracked = false)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if(!tracked)
                query = query.AsNoTracking();

            if (filter is not null)
                query= query.Where(filter);

            if (includes is not null && includes.Length > 0)
            {
                foreach (var inc in includes)
                {
                    query = query.Include(inc);
                }
            }

            return await query.ToListAsync();

        }
    }
}

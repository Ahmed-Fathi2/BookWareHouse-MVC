using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey>(ApplicationDbContext dbContext) : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
          return  await  _dbContext.Set<TEntity>().ToListAsync();
        }

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
    }
}

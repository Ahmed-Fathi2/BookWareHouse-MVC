using BookWarehouse.Domain.Entities;
using BookWarehouse.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Seeders
{
    public class CategorySeeder(ApplicationDbContext dbContext) : ICategorySeeder
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task SeedAsync()
        {
            if (await _dbContext.Database.CanConnectAsync())
            {
                if (!_dbContext.Categories.Any())
                {
                    var categories = GetCategories();
                     _dbContext.AddRange(categories); // Add In Local Container So No Need for Async Version of AddRange
                    await _dbContext.SaveChangesAsync();
                }
            }
        }


        private IEnumerable<Category> GetCategories()
        {

            var categories = new List<Category>()
            {
                    new Category { Name = "Novels",  DisplayOrder = 1 },
                    new Category { Name = "Children's Books", DisplayOrder = 2 },
                    new Category { Name = "Science", DisplayOrder = 3 },
                    new Category { Name = "History", DisplayOrder = 4 },
                    new Category { Name = "Religion & Philosophy", DisplayOrder = 5 },
                    new Category { Name = "Self-Help",  DisplayOrder = 6 }
            };

            return categories;
        }
    }
}

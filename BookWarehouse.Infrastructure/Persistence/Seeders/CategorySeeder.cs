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
                new Category
                {
                    Name = "Novels",
                    DisplayOrder = 1,
                    Description = "Fictional stories and literary works."
                },
                new Category
                {
                    Name = "Children's Books",
                    DisplayOrder = 2,
                    Description = "Books designed for kids with simple and fun content."
                },
                new Category
                {
                    Name = "Science",
                    DisplayOrder = 3,
                    Description = "Books covering scientific topics and discoveries."
                },
                new Category
                {
                    Name = "History",
                    DisplayOrder = 4,
                    Description = "Books about historical events and civilizations."
                },
                new Category
                {
                    Name = "Religion & Philosophy",
                    DisplayOrder = 5,
                    Description = "Books discussing religious and philosophical ideas."
                },
                new Category
                {
                    Name = "Self-Help",
                    DisplayOrder = 6,
                    Description = "Books that help improve personal development."
                },
                new Category
                {
                    Name = "Technology",
                    DisplayOrder = 7,
                    Description = "Books related to computer science and modern technology."
                },
                new Category
                {
                    Name = "Biographies",
                    DisplayOrder = 8,
                    Description = "Detailed descriptions of a person's life."
                },
                new Category
                {
                    Name = "Fantasy",
                    DisplayOrder = 9,
                    Description = "Books containing magical and supernatural elements."
                },
                new Category
                {
                    Name = "Mystery & Thriller",
                    DisplayOrder = 10,
                    Description = "Books filled with suspense, crimes, and unexpected plot twists."
                },
                new Category
                {
                    Name = "Business & Economics",
                    DisplayOrder = 11,
                    Description = "Books about finance, markets, and entrepreneurship."
                },
                new Category
                {
                    Name = "Cooking & Food",
                    DisplayOrder = 12,
                    Description = "Cookbooks, recipes, and culinary history."
                }
            };

            return categories;
        }
    }
}

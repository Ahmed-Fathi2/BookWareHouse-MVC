using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Seeders
{
    public class DataBaseSeeder(ICategorySeeder categorySeeder) : IDataBaseSeeder
    {
        private readonly ICategorySeeder _categorySeeder = categorySeeder;

        public async Task SeedAsync()
        {
            await _categorySeeder.SeedAsync();
        }
    }
}

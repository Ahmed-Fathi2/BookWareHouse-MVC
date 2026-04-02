using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Seeders
{
    public class DataBaseSeeder(ICategorySeeder categorySeeder,IRoleSeeder roleSeeder) : IDataBaseSeeder
    {
        private readonly ICategorySeeder _categorySeeder = categorySeeder;
        private readonly IRoleSeeder _roleSeeder = roleSeeder;

        public async Task SeedAsync()
        {
            await _categorySeeder.SeedAsync();
            await _roleSeeder.SeedAsync();
        }
    }
}

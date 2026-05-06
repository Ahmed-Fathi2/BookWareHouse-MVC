using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Seeders
{
    public class DataBaseSeeder(ICategorySeeder categorySeeder, IRoleSeeder roleSeeder, IProductSeeder productSeeder) : IDataBaseSeeder
    {
        private readonly ICategorySeeder _categorySeeder = categorySeeder;
        private readonly IRoleSeeder _roleSeeder = roleSeeder;
        private readonly IProductSeeder _productSeeder = productSeeder;

        public async Task SeedAsync()
        {
            await _roleSeeder.SeedAsync();
            await _categorySeeder.SeedAsync();
            await _productSeeder.SeedAsync();
        }
    }
}

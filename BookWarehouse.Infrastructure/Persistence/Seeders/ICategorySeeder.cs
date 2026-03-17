using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Seeders
{
    public interface ICategorySeeder
    {
        Task SeedAsync();
    }
}

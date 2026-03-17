using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Seeders
{
    public interface IDataBaseSeeder
    {
        Task SeedAsync();
    }
}

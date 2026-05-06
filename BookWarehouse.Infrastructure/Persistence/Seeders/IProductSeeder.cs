using System.Threading.Tasks;

namespace BookWarehouse.Infrastructure.Persistence.Seeders
{
    public interface IProductSeeder
    {
        Task SeedAsync();
    }
}

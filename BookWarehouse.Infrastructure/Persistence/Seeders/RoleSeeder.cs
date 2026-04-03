using BookWarehouse.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;

namespace BookWarehouse.Infrastructure.Persistence.Seeders
{
    public class RoleSeeder(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager) : IRoleSeeder
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public async Task SeedAsync()
        {
            var roles = new[] { "Admin", "Customer" };
            if (await _dbContext.Database.CanConnectAsync())
            {
                if (!_dbContext.Roles.Any())
                {
                    foreach (var role in roles)
                    {
                        if (!await _roleManager.RoleExistsAsync(role))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }
        
                }
            }

        }
    }
}

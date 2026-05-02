using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookWarehouse.Infrastructure.Services.Auth
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return roles!;
        }

        public async Task<Result> CreateRoleAsync(string roleName)
        {
            //if (string.IsNullOrWhiteSpace(roleName))
            //{
            //    return Result.Failure(new Error("Role.Empty", "Role name cannot be empty."));
            //}

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                return Result.Failure(new Error("Role.Exists", $"Role '{roleName}' already exists."));
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return Result.Success();
            }
           var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description));
        }
    }
}

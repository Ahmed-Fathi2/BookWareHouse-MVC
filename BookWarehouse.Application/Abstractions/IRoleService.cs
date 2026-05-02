using BookWarehouse.Application.Comman.Results;
using System.Threading.Tasks;

namespace BookWarehouse.Application.Abstractions
{
    public interface IRoleService
    {
        Task<IEnumerable<string>> GetAllRolesAsync();
        Task<Result> CreateRoleAsync(string roleName);
    }
}

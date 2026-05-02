using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookWarehouse.Application.Abstractions
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserListVM>> GetAllUsersAsync();
        Task<Result> CreateUserAsync(CreateUserVM model);
        Task<Result> DeleteUserAsync(string userId);
    }
}

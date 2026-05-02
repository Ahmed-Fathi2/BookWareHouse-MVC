using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.User;
using BookWarehouse.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookWarehouse.Infrastructure.Services.Auth
{
    public class UserManagementService(UserManager<ApplicationUser> userManager) : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<IEnumerable<UserListVM>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync(); // you can use _userMAnager as dbContext.Users.ToListAsync() 
            var userList = new List<UserListVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserListVM
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    Roles = roles
                });
            }

            return userList;
        }

        public async Task<Result> CreateUserAsync(CreateUserVM model)
        {

            var isUserExist = await _userManager.FindByEmailAsync(model.Email);
            if (isUserExist is not null) 
                return Result.Failure(new Error("User.AlreadyExists", "A user with this email already exists."));


            var user = model.Adapt<ApplicationUser>();
            user.UserName = model.Email; 

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user); 
                    return Result.Failure(new Error("User.RoleAssignmentFailed", string.Join(", ", roleResult.Errors.Select(e => e.Description))));
                }

                return Result.Success();
            }
            
            return Result.Failure(new Error("User.CreateFailed", string.Join(", ", result.Errors.Select(e => e.Description))));
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure(new Error("User.NotFound", "User not found."));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Result.Success();
            }

            return Result.Failure(new Error("User.DeleteFailed", string.Join(", ", result.Errors.Select(e => e.Description))));
        }
    }
}

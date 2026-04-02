using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Constants;
using BookWarehouse.Application.Comman.Errors.User;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Domain.Entities;
using Ecom.BLL.ViewModel.Authentication;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace BookWarehouse.Application.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Result> RegisterAsync(RegisterVM registerVM)
        {

            var user= await _userManager.FindByEmailAsync(registerVM.Email);
            if(user is not null)
                return Result.Failure(UserErrors.EmailAlreadyExists);

            var newUser = registerVM.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if(!result.Succeeded)
                return Result.Failure(UserErrors.UserCreationFailed);

            result = await _userManager.AddToRoleAsync(newUser, DefaultRole.User);
            if (!result.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                return Result.Failure(UserErrors.UserCreationFailed);
            }

            return Result.Success();

        }
    }
}

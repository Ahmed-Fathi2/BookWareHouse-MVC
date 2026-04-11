using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Constants;
using BookWarehouse.Application.Comman.Errors.User;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Auth;
using BookWarehouse.Domain.Entities;
using Ecom.BLL.ViewModel.Authentication;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace BookWarehouse.Infrastructure.Services.Auth
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManger) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManger = signInManger;

        public async Task<Result> RegisterAsync(RegisterVM registerVM)
        {

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user is not null)
                return Result.Failure(UserErrors.EmailAlreadyExists);

            var newUser = registerVM.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (!result.Succeeded)
                return Result.Failure(UserErrors.UserCreationFailed);

            result = await _userManager.AddToRoleAsync(newUser, DefaultRole.Customer);
            if (!result.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                return Result.Failure(UserErrors.UserCreationFailed);
            }

            return Result.Success();

        }
        public async Task<Result> LoginAsync(LoginVM loginVM)
        {


            var user = await _userManager.FindByEmailAsync(loginVM.Email);

            if (user is null)
                return Result.Failure(UserErrors.InvalidCredentials);

            var result = await _signInManger.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, true);

            if (!result.Succeeded)
            {
                return result.IsNotAllowed ? Result.Failure(UserErrors.EmailNotConfirmed) :
                       result.IsLockedOut ? Result.Failure(UserErrors.UserLockedOut) :
                       Result.Failure(UserErrors.InvalidCredentials);
            }

            return Result.Success();
        }

        public async Task<Result> LogoutAsync()
        {
            await _signInManger.SignOutAsync();
            return Result.Success();

        }
    }
}

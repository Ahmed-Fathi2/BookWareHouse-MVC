using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Auth;
using Ecom.BLL.ViewModel.Authentication;

namespace BookWarehouse.Application.Abstractions
{
    public interface IAuthService
    {

        Task<Result> RegisterAsync(RegisterVM registerVM);
        Task<Result> LoginAsync(LoginVM loginVM);

        Task<Result> LogoutAsync();
    }
}

using BookWarehouse.Application.Comman.Results;
using Ecom.BLL.ViewModel.Authentication;

namespace BookWarehouse.Application.Abstractions
{
    public interface IAuthService
    {

        Task<Result> RegisterAsync(RegisterVM registerVM);
        //Task<Result> LoginAsync();
        //Task<Result> LogoutAsync();
    }
}

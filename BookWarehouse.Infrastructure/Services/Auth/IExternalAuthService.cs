using BookWarehouse.Application.Comman.Results;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace BookWarehouse.Infrastructure.Services.Auth
{
    public interface IExternalAuthService
    {
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
        Task<Result> ExternalLoginCallbackAsync(string? remoteError = null);
    }
}

using BookWarehouse.Application.Comman.Results;
using Microsoft.AspNetCore.Authentication;

namespace BookWarehouse.Application.Abstractions
{
    public interface IExternalAuthService
    {
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
        Task<Result> ExternalLoginCallbackAsync(string? remoteError = null);
    }
}

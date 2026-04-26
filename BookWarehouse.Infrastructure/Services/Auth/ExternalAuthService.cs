using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Constants;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookWarehouse.Infrastructure.Services.Auth
{
    public class ExternalAuthService : IExternalAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExternalAuthService> _logger;

        public ExternalAuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalAuthService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }

        public async Task<Result> ExternalLoginCallbackAsync(string? remoteError = null)
        {
            if (remoteError != null)
            {
                return Result.Failure(new Error("ExternalProviderError", $"Error from external provider: {remoteError}"));
            }
            // Retrieve the external login info from the authentication middleware
            // info stored in the cookie after the user is redirected back from the external provider
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return Result.Failure(new Error("ExternalLoginInfoNotFound", "Could not retrieve external login info."));
            }

            //foreach (var claim in externalLoginInfo.Principal.Claims)
            //{
            //    _logger.LogInformation("Claim Type: {Type} - Value: {Value}", claim.Type, claim.Value);
            //}

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return Result.Success();
            }

            var userEmail = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            if (userEmail != null)
            {
                var existingUser = await _userManager.FindByEmailAsync(userEmail);

                var firstName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
                var lastName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

                if (existingUser == null)
                {
                    existingUser = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        FirstName = firstName,
                        LastName = lastName,
                        EmailConfirmed = true
                    };

                    var creationResult = await _userManager.CreateAsync(existingUser);
                    if (!creationResult.Succeeded)
                    {
                        return Result.Failure(new Error("UserCreationFailed", "Failed to create user account from external login."));
                    }

                  var roleResult = await _userManager.AddToRoleAsync(existingUser, DefaultRole.Customer);
                    if (!roleResult.Succeeded)
                    {
                        return Result.Failure(new Error("RoleAssignmentFailed", "Failed to assign default role to the user account."));
                    }
                }
                else
                {
                    bool isUpdated = false;

                    if (string.IsNullOrWhiteSpace(existingUser.FirstName) &&
                        !string.IsNullOrWhiteSpace(firstName))
                    {
                        existingUser.FirstName = firstName;
                        isUpdated = true;
                    }

                    if (string.IsNullOrWhiteSpace(existingUser.LastName) &&
                        !string.IsNullOrWhiteSpace(lastName))
                    {
                        existingUser.LastName = lastName;
                        isUpdated = true;
                    }

                    if (!existingUser.EmailConfirmed)
                    {
                        existingUser.EmailConfirmed = true;
                        isUpdated = true;
                    }

                    if (isUpdated)
                    {
                        await _userManager.UpdateAsync(existingUser);
                    }
                }


                var addLoginResult = await _userManager.AddLoginAsync(existingUser, externalLoginInfo);
                if (!addLoginResult.Succeeded)
                {
                    return Result.Failure(new Error("ExternalLoginAddFailed", "Failed to link external login to the user account."));
                }

                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                return Result.Success();
            }

            return Result.Failure(new Error("EmailClaimMissing", "Email claim is missing from the external provider."));
        }
    }
}

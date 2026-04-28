using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Constants;
using BookWarehouse.Application.Comman.Errors.User;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Auth;
using BookWarehouse.Domain.Entities;
using Ecom.BLL.ViewModel.Authentication;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

namespace BookWarehouse.Infrastructure.Services.Auth
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManger, IEmailService emailService) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManger = signInManger;
        private readonly IEmailService _emailService = emailService;

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
        public async Task<Result> ExternalLoginCallbackAsync(string? remoteError = null)
        {
            if (remoteError != null)
            {
                return Result.Failure(new Error("ExternalProviderError", $"Error from external provider: {remoteError}"));
            }
            // Retrieve the external login info from the authentication middleware
            // info stored in the cookie after the user is redirected back from the external provider
            var externalLoginInfo = await _signInManger.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return Result.Failure(new Error("ExternalLoginInfoNotFound", "Could not retrieve external login info."));
            }

            //foreach (var claim in externalLoginInfo.Principal.Claims)
            //{
            //    _logger.LogInformation("Claim Type: {Type} - Value: {Value}", claim.Type, claim.Value);
            //}

            var signInResult = await _signInManger.ExternalLoginSignInAsync(
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

                await _signInManger.SignInAsync(existingUser, isPersistent: false);
                return Result.Success();
            }

            return Result.Failure(new Error("EmailClaimMissing", "Email claim is missing from the external provider."));
        }


        public async Task<Result> LogoutAsync()
        {
            await _signInManger.SignOutAsync();
            return Result.Success();

        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordVM forgotPasswordVM, string origin)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

            if (user is null)
                return Result.Success();


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var resetLink = $"{origin}/Auth/ResetPassword?email={user.Email}&token={token}";
            //var resetLink = " https://www.youtube.com/";
            var body = $@"
                            <html>
                            <head>
                                <style>
                                    .container {{
                                        font-family: Arial;
                                        padding: 20px;
                                        background-color: #f4f4f4;
                                    }}
                                    .card {{
                                        background: white;
                                        padding: 20px;
                                        border-radius: 10px;
                                    }}
                                    .btn {{
                                        display: inline-block;
                                        padding: 10px 20px;
                                        background-color: #007bff;
                                        color: white !important;
                                        text-decoration: none;
                                        border-radius: 5px;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <div class='card'>
                                        <h2>Password Reset Request</h2>
                                        <p>Hello {user.FirstName},</p>
                                        <p>You requested to reset your password. Click the button below:</p>

                                        <a class='btn' href='{resetLink}'>Reset Password</a>

                                        <p style='margin-top:20px; font-size:12px; color:gray;'>
                                            If you didn’t request this, ignore this email.
                                        </p>
                                    </div>
                                </div>
                            </body>
                            </html>";


            await _emailService.SendEmailAsync(
                user.Email!,
                $"{user.FirstName} {user.LastName}",
                "Password Reset Request",
                body
            );


            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordVM resetPasswordVM)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordVM.Email);

            if (user is null)
                return Result.Failure(new Error("UserNotFound", "User not found."));


            var decodedToken = resetPasswordVM.Token;

            try
            {

               decodedToken= Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordVM.Token));
            }
            catch (FormatException)
            {
                return Result.Failure(new Error("InvalidToken", "The provided token is not valid."));
            }

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordVM.NewPassword);
            if (!result.Succeeded)
            {
                return Result.Failure(new Error("PasswordResetFailed", "Failed to reset the password."));
            }

            return Result.Success();
        }
    }
}

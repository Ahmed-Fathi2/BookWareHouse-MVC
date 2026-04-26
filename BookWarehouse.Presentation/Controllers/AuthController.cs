using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Auth;
using BookWarehouse.Domain.Entities;
using Ecom.BLL.ViewModel.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookWarehouse.Presentation.Controllers
{
    public class AuthController(IAuthService authService,
            SignInManager<ApplicationUser> signInManager
             ) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            var result = await _authService.RegisterAsync(registerVM);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error.Description);
                return View(registerVM);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }


            var result = await _authService.LoginAsync(loginVM);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error.Description);
                return View(loginVM);
            }
            return RedirectToAction(nameof(LoginRedirect));
        }

        [HttpGet]
        public IActionResult LoginRedirect()
        {
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                return RedirectToAction("Index", "Order");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var result = await _authService.ExternalLoginCallbackAsync(remoteError);

            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(LoginRedirect));
            }

            ModelState.AddModelError(string.Empty, result.Error.Description);

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordVM);
            }

            var origin = $"{Request.Scheme}://{Request.Host}";
            var result = await _authService.ForgotPasswordAsync(forgotPasswordVM, origin);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error.Description);
                return View(forgotPasswordVM);
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (token == null || email == null)
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }
            var result = await _authService.ResetPasswordAsync(resetPasswordVM);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Error.Description);
                return View(resetPasswordVM);
            }
            return RedirectToAction(nameof(Login));
        }
    }
}
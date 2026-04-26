using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Auth;
using Ecom.BLL.ViewModel.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Application.Comman.Constants;
using BookWarehouse.Infrastructure.Services.Auth;

namespace BookWarehouse.Presentation.Controllers
{
    public class AuthController(IAuthService authService, IExternalAuthService externalAuthService) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly IExternalAuthService _externalAuthService = externalAuthService;

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
            var properties = _externalAuthService.ConfigureExternalAuthenticationProperties(provider, redirectUrl!);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            
            var result = await _externalAuthService.ExternalLoginCallbackAsync(remoteError);

            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(LoginRedirect));
            }
            
            ModelState.AddModelError(string.Empty, result.Error.Description);
            return RedirectToAction(nameof(Login));
        }
    }
}

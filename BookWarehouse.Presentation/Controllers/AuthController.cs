using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Auth;
using Ecom.BLL.ViewModel.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace BookWarehouse.Presentation.Controllers
{
    public class AuthController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService = authService;

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
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }

    }
}

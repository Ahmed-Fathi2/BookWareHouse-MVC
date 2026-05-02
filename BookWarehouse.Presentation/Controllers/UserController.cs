using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Constants;
using BookWarehouse.Application.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookWarehouse.Presentation.Controllers
{
    [Authorize(Roles = DefaultRole.Admin)]
    public class UserController : Controller
    {
        private readonly IUserManagementService _userService;
        private readonly IRoleService _roleService;

        public UserController(IUserManagementService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleService.GetAllRolesAsync();
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserVM model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _roleService.GetAllRolesAsync();
                ViewBag.Roles = roles;
                return View(model);
            }

            var result = await _userService.CreateUserAsync(model);
            if (result.IsSuccess)
            {
                TempData["success"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Error.Description);
            var rolesAgain = await _roleService.GetAllRolesAsync();
            ViewBag.Roles = rolesAgain;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result.IsSuccess)
            {
                TempData["success"] = "User deleted successfully.";
            }
            else
            {
                TempData["error"] = result.Error.Description;
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}

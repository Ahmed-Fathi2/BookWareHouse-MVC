using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Constants;
using BookWarehouse.Application.ViewModels.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookWarehouse.Presentation.Controllers
{
    [Authorize(Roles = DefaultRole.Admin)]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _roleService.CreateRoleAsync(model.RoleName);
            if (result.IsSuccess)
            {
                TempData["success"] = $"Role '{model.RoleName}' created successfully.";
                return RedirectToAction("Index", "DashBoard"); 
            }

            ModelState.AddModelError(string.Empty, result.Error.Description);
            return View(model);
        }
    }
}

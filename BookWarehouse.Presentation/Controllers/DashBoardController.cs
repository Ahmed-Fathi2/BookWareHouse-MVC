using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookWarehouse.Presentation.Controllers
{
    [Authorize(Roles = DefaultRole.Admin)]
    public class DashBoardController(IDashboardService dashboardService) : Controller
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        public async Task<IActionResult> Index()
        {
            var dashboardStats = await _dashboardService.GetDashboardStatsAsync();
            return View(dashboardStats);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDetails()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDetailsData()
        {
            var customerDetails = await _dashboardService.GetCustomerDetailsAsync();

            return Json(new {data= customerDetails });
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace BookWarehouse.Presentation.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

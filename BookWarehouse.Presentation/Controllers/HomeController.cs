using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Application.ViewModels.Home;
using BookWarehouse.Application.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace BookWarehouse.Presentation.Controllers
{
    public class HomeController(IProductService productService, ICategoryService categoryService) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly ICategoryService _categoryService = categoryService;

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                return RedirectToAction("Index", "DashBoard");
            }

            var categories = await _categoryService.GetAllCategories();
            var products = await _productService.GetAllProducts(new ProductQueryVM { PageSize = 8, PageNumber = 1 });

            var vm = new HomeIndexVM
            {
                Categories = categories.IsSuccess ? categories.Value : new List<CategoryReadEditVM>(),
                FeaturedProducts = products.IsSuccess ? products.Value.Items : new List<ProductReadVM>()
            };

            return View(vm);
        }
    }
}

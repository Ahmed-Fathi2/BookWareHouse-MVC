using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Application.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace BookWarehouse.Presentation.Controllers
{
    public class CustomerProductController( IProductService productService, ICategoryService categoryService) : Controller
    {

        private readonly IProductService _productService = productService;
        private readonly ICategoryService _categoryService = categoryService;


        [HttpGet]
        public async Task<IActionResult> Index(ProductQueryVM productQueryVM)
        {
            var result = await _productService.GetAllProducts(productQueryVM);
            var categories = await _categoryService.GetAllCategories();
            
            var vm = new CustomerProductIndexVM
            {
                Query = productQueryVM,
                Categories = categories.IsSuccess ? categories.Value : new List<CategoryReadEditVM>(),
                Products = result.Value
            };
           
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _productService.GetProductById(id);

            if (!result.IsSuccess)
                return NotFound();

            var productReadVM = result.Value;
            return View(productReadVM);
        }
    }
}

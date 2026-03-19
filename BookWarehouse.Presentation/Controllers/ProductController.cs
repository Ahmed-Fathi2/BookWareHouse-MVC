using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookWarehouse.Presentation.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public ProductController(ICategoryService categoryService,IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _productService.GetAllProducts();
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _productService.GetProductById(id);
            if (!result.IsSuccess)
                return NotFound();
            var productReadVM = result.Value;
            return View(productReadVM);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var categories =await GetSelectListItemOfCategories();
            ViewBag.Categories = categories;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> SaveCreate(ProductCreateVM productCreateVM)
        {
            if (!ModelState.IsValid)
            {
                var categories = await GetSelectListItemOfCategories();
                ViewBag.Categories = categories;
                return View("Create", productCreateVM);
            }

            await _productService.CreateProduct(productCreateVM);
            return RedirectToAction("Index");


        }

        private async Task<IEnumerable<SelectListItem>> GetSelectListItemOfCategories()
        {
            var result = await _categoryService.GetAllCategories();
            var categories = result.Value.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });

            return categories;
        }
    }
}

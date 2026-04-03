using BookWarehouse.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BookWarehouse.Presentation.Controllers
{
    public class CustomerProductController( IProductService productService) : Controller
    {

        private readonly IProductService _productService = productService;


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
    }
}

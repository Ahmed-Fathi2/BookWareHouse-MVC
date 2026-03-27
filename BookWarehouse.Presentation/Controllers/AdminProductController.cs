using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Product;
using BookWarehouse.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace BookWarehouse.Presentation.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _environment;

        public AdminProductController(ICategoryService categoryService, IProductService productService,IFileService fileService,IWebHostEnvironment environment)
        {
            _categoryService = categoryService;
            _productService = productService;
            _fileService = fileService;
            _environment = environment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //var result = await _productService.GetAllProducts();
            return View();
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

            var categories = await GetSelectListItemOfCategories();
            ViewBag.Categories = categories;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> SaveCreate(ProductCreateVM productCreateVM, IFormFile? image)
        {
            if (image is not null && image.Length != 0)
            {
                var maxFileSize = 5 * 1024 * 1024;
                if (image.Length > maxFileSize)
                {
                    ModelState.AddModelError("Image", "Max allowed size is 5MB.");

                }
            }
            else
            {
                ModelState.AddModelError("Image", "Image is required and must have a size greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                var categories = await GetSelectListItemOfCategories();
                ViewBag.Categories = categories;
                return View("Create", productCreateVM);
            }

            var webRootPath = _environment.WebRootPath; // Path to wwwroot folder
            using var imageStream = image!.OpenReadStream();


            var fileUploadResult = await _fileService.Upload(webRootPath, image!.FileName, imageStream);

            productCreateVM.ImageUrl = fileUploadResult.Value;

            await _productService.CreateProduct(productCreateVM);

            TempData["success"] = "Book Created successfully.";
            return RedirectToAction("Index");


        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {


            var product = await _productService.GetProductForEdit(id);

            if (product == null)
                return NotFound();
            
            var categories = await GetSelectListItemOfCategories();
            ViewBag.Categories = categories;
            return View(product.Value);

        }
        [HttpPost]
        public async Task<IActionResult> SaveEdit(ProductEditVM productEditVM, IFormFile? newImage)
        {
            if (newImage is not null && newImage.Length != 0)
            {
                var maxFileSize = 5 * 1024 * 1024;
                if (newImage.Length > maxFileSize)
                {
                    ModelState.AddModelError("Image", "Max allowed size is 5MB.");
                }
            }

            if (!ModelState.IsValid)
            {
                var categories = await GetSelectListItemOfCategories();
                ViewBag.Categories = categories;
                return View("Edit", productEditVM);
            }


            var webRootPath = _environment.WebRootPath;
            using var imageStream = newImage?.OpenReadStream();

            var result = await _productService.UpdateProduct(productEditVM, webRootPath, newImage?.FileName, imageStream);
            if (!result.IsSuccess)
                return NotFound();

            TempData["success"] = "Book Updated successfully.";
            return RedirectToAction("Index");
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {

            var webRootPath = _environment.WebRootPath;

            var result = await _productService.DeleteProduct(id, webRootPath);

            if (!result.IsSuccess)
                return NotFound();

            //TempData["success"] = "Book Deleted successfully.";


            return Json(new { success = true, message = "Book Deleted successfully." });

        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllProducts();
            return Json(new {data=result.Value });
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

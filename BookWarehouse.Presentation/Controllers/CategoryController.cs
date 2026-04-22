using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Application.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace BookWarehouse.Presentation.Controllers
{
    public class CategoryController(ICategoryService categoryService , IWebHostEnvironment environment , IFileService fileService) : Controller
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly IFileService _fileService = fileService;

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategories();
            return View(categories.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCreate(CategoryCreateVM categoryCreateVM , IFormFile? image)
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
                ModelState.AddModelError("Image", "Image is required and must have a size less than 5MB.");
            }


            if (!ModelState.IsValid)
            {
                return View("Create", categoryCreateVM);
            }


            var webRootPath = _environment.WebRootPath; // Path to wwwroot folder
            using var imageStream = image!.OpenReadStream();


            var fileUploadResult = await _fileService.Upload(webRootPath, image!.FileName, imageStream);

            categoryCreateVM.ImageUrl = fileUploadResult.Value;

            await _categoryService.CreateCategory(categoryCreateVM);

            TempData["success"] = "Category Created successfully.";

            return RedirectToAction("Index");



        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var result = await _categoryService.GetCategoryById(id);

            if (!result.IsSuccess)
                return NotFound();

            var categoryReadEditVM = result.Value;

            return View(categoryReadEditVM);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(CategoryReadEditVM categoryReadEditVM, IFormFile? image)
        {
            if (image is not null && image.Length != 0)
            {
                var maxFileSize = 5 * 1024 * 1024;
                if (image.Length > maxFileSize)
                {
                    ModelState.AddModelError("Image", "Max allowed size is 5MB.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", categoryReadEditVM);
            }

            if (image is not null && image.Length > 0)
            {
                var webRootPath = _environment.WebRootPath;

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(categoryReadEditVM.ImageUrl))
                {
                    var oldImagePath = Path.Combine(webRootPath, categoryReadEditVM.ImageUrl.TrimStart('\\', '/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using var imageStream = image.OpenReadStream();
                var fileUploadResult = await _fileService.Upload(webRootPath, image.FileName, imageStream);
                categoryReadEditVM.ImageUrl = fileUploadResult.Value;
            }

            var result = await _categoryService.UpdateCategory(categoryReadEditVM);

            if (!result.IsSuccess)
                return NotFound();

            TempData["success"] = "Category Updated successfully.";


            return RedirectToAction("Index");

        }

        //[HttpGet]

        //public async Task<IActionResult> Delete(int id)
        //{

        //    var result = await _categoryService.GetCategoryById(id);

        //    if (!result.IsSuccess)
        //        return NotFound();

        //    var categoryReadEditVM = result.Value;

        //    return View(categoryReadEditVM);

        //}

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
          

            var result = await _categoryService.DeleteCategory(id);

            if (!result.IsSuccess)
                return NotFound();

            TempData["success"] = "Category Deleted successfully.";


            return Json(new { success = true });

        }

    }
}

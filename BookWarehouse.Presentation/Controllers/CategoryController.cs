using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace BookWarehouse.Presentation.Controllers
{
    public class CategoryController(ICategoryService categoryService) : Controller
    {
        private readonly ICategoryService _categoryService = categoryService;

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
        public async Task<IActionResult> SaveCreate(CategoryCreateVM categoryCreateVM)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", categoryCreateVM);
            }
           

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
        public async Task<IActionResult> SaveEdit(CategoryReadEditVM categoryReadEditVM)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", categoryReadEditVM);
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

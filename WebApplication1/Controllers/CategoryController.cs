using Microsoft.AspNetCore.Mvc;
using Project.Application.DTOs;
using Project.Application.Interfaces;

namespace Project.MVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService _categoryService)
        {
            this._categoryService = _categoryService;
        }
        public ActionResult Index() 
        {
            var categories = _categoryService.GetAllCategories();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategoryView()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CreateCategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateCategoryView", category);
            }

            var created = _categoryService.CreateCategory(category);

            if (!created)
            {
                ModelState.AddModelError("Name", "Category already exists");

                return View("CreateCategoryView", category);
            }

            return RedirectToAction("Index");
        }
    }
}

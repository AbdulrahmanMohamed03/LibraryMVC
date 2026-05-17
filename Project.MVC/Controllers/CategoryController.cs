using Microsoft.AspNetCore.Mvc;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Category;
using System.Security.Claims;

namespace Project.MVC.Controllers
{
    // [Authorize] 
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }


        private string ActorId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Admin_User";


        [HttpGet]
        public IActionResult Index(string? search, bool showDeleted = false)
        {

            var model = _categoryService.GetIndexViewModel(search, showDeleted);
            return View(model);
        }


        [HttpGet]
        // [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _categoryService.Create(model, ActorId);
                TempData["SuccessMessage"] = $"Category '{model.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category.");
                ModelState.AddModelError("Name", ex.Message);
                return View(model);
            }
        }


        [HttpGet]
        // [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Edit(int id)
        {
            try
            {
                var model = _categoryService.GetFormViewModel(id);
                return View(model);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _categoryService.Update(model, ActorId);
                TempData["SuccessMessage"] = $"Category '{model.Name}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {Id}", model.Id);
                ModelState.AddModelError("Name", ex.Message);
                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {

                var category = _categoryService.GetById(id);
                _categoryService.Delete(id, ActorId);
                TempData["SuccessMessage"] = $"Category '{category.Name}' deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Restore(int id)
        {
            _categoryService.Restore(id, ActorId);
            TempData["SuccessMessage"] = "Category restored successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult IsNameAvailable(string name, int id)
        {
            bool exists = _categoryService.GetIndexViewModel().Categories
                          .Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Id != id);

            return Json(!exists);
        }
        //for form category  in add newbook modal
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult CreateModal(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Category name is required.");

            try
            {
                var model = new CategoryFormViewModel { Name = name };
                var result = _categoryService.Create(model, ActorId);
                return Json(new { id = result.Id, text = result.Name });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category from modal.");
                return BadRequest(ex.Message);
            }
        }
    }
}

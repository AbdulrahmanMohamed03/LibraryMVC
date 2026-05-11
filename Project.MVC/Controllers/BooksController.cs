// Project.Web/Controllers/BooksController.cs
using Microsoft.AspNetCore.Mvc;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Book;

namespace Project.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _service;
        private readonly IWebHostEnvironment _env;         

        public BooksController(IBookService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;                           
        }

        public IActionResult Index()
            => View(new BookIndexViewModel { Books = _service.GetAll() });

        public IActionResult Details(int id)
        {
            var vm = _service.GetById(id);
            return vm is null ? NotFound() : View(vm);
        }

        public IActionResult Create()
            => View(_service.GetCreateForm());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(BookFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var form = _service.GetCreateForm();
                vm.Authors = form.Authors;
                vm.Categories = form.Categories;
                return View(vm);
            }

            _service.Create(vm, _env.WebRootPath);         
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var vm = _service.GetEditForm(id);
            return vm is null ? NotFound() : View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(BookFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var form = _service.GetEditForm(vm.Id) ?? _service.GetCreateForm();
                vm.Authors = form.Authors;
                vm.Categories = form.Categories;
                return View(vm);
            }

            try
            {
                var result = _service.Update(vm, _env.WebRootPath);  
                return result is null ? NotFound() : RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(vm.TotalCopies), ex.Message);
                var form = _service.GetEditForm(vm.Id) ?? _service.GetCreateForm();
                vm.Authors = form.Authors;
                vm.Categories = form.Categories;
                return View(vm);
            }
        }

        public IActionResult Delete(int id)
        {
            var vm = _service.GetById(id);
            return vm is null ? NotFound() : View(vm);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // delete image file from disk before deleting the book
            var book = _service.GetById(id);
            if (book is not null && !string.IsNullOrEmpty(book.CoverImageUrl))
            {
                var filePath = Path.Combine(
                    _env.WebRootPath,
                    book.CoverImageUrl.TrimStart('/')
                );
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            var deleted = _service.Delete(id);
            return deleted ? RedirectToAction(nameof(Index)) : NotFound();
        }
    }
}
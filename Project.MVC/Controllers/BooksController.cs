


// Project.Web/Controllers/BooksController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Book;
using Project.Core.Models;
using System.IO;
using System.Linq;

namespace Project.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _service;
        private readonly IWebHostEnvironment _env;
        private readonly IReservationService _reservationService;
        private readonly IBorrowingService _borrowingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(
            IBookService service,
            IWebHostEnvironment env,
            IReservationService reservationService,
            IBorrowingService borrowingService,
            UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _env = env;
            _reservationService = reservationService;
            _borrowingService = borrowingService;
            _userManager = userManager;
        }

        // ─────────────────────────────────────────────────────────────────────
        [AllowAnonymous]
        public IActionResult Index(int? categoryId, string searchString)
        {
      
            var allBooks = _service.GetAll().ToList();

    
  
            var categoryList = allBooks
                .GroupBy(b => new { b.CategoryId, b.CategoryName })
                .Select(g => new { Id = g.Key.CategoryId, Name = g.Key.CategoryName, Count = g.Count() })
                .OrderBy(c => c.Name)
                .ToList();

            ViewBag.Categories = categoryList;   

           
            var filtered = allBooks.AsEnumerable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                filtered = filtered.Where(b => b.CategoryId == categoryId.Value);
           
                ViewBag.ActiveCategoryName = categoryList
                    .FirstOrDefault(c => c.Id == categoryId.Value)?.Name;
            }

        
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                filtered = filtered.Where(b =>
                    (!string.IsNullOrEmpty(b.Title) && b.Title.ToLower().Contains(term)) ||
                    (!string.IsNullOrEmpty(b.AuthorName) && b.AuthorName.ToLower().Contains(term)) ||
                    (!string.IsNullOrEmpty(b.ISBN) && b.ISBN.Contains(term)));
            }

      
            var viewModel = new BookIndexViewModel
            {
                Books = filtered.ToList(),
                SearchTerm = searchString,     
                FilterCategoryId = categoryId        
            };

            return View(viewModel);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DETAILS
        // ─────────────────────────────────────────────────────────────────────
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var vm = _service.GetById(id);
            if (vm is null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                vm.IsCurrentlyBorrowing = _borrowingService.UserHasActiveBorrowForBook(userId, id);
                vm.HasActiveReservation = _reservationService.UserHasActiveReservationForBook(userId, id);
            }

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────────────────
        // CREATE / EDIT / DELETE  (Admin / Librarian only — logic unchanged)
        // ─────────────────────────────────────────────────────────────────────
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create()
            => View(_service.GetCreateForm());

        [Authorize(Roles = "Admin,Librarian")]
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

        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Edit(int id)
        {
            var vm = _service.GetEditForm(id);
            return vm is null ? NotFound() : View(vm);
        }

        [Authorize(Roles = "Admin,Librarian")]
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

        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Delete(int id)
        {
            var vm = _service.GetById(id);
            return vm is null ? NotFound() : View(vm);
        }

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var book = _service.GetById(id);
            if (book is null) return NotFound();

            if (book.AvailableCopies > 0)
            {
                ModelState.AddModelError("", "Cannot delete a book that still has available copies.");
                return View("Delete", book);
            }

            if (!string.IsNullOrEmpty(book.CoverImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, book.CoverImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            return _service.Delete(id) ? RedirectToAction(nameof(Index)) : NotFound();
        }

        [HttpGet]
        public IActionResult IsISBNAvailable(string isbn, int id = 0)
            => Json(!_service.ISBNExists(isbn, excludeId: id));
    }
}
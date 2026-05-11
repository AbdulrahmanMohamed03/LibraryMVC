// Project.Application/Services/Implementation/BookService.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Book;
using Project.Core;
using Project.Core.Models;

namespace Project.Application.Services.Implementaion
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _uow;

        public BookService(IUnitOfWork uow) => _uow = uow;

        // ── Helpers ──────────────────────────────────────────────────────────

        private static BookViewModel ToViewModel(Book b) => new BookViewModel
        {
            Id = b.Id,
            ISBN = b.ISBN,
            Title = b.Title,
            Description = b.Description,
            AuthorId = b.AuthorId,
            AuthorName = b.Author?.FullName ?? "—",
            CategoryId = b.CategoryId,
            CategoryName = b.Category?.Name ?? "—",
            PublishedYear = b.PublishedYear,
            BorrowFee = b.BorrowFee,
            DailyFineRate = b.DailyFineRate,
            TotalCopies = b.TotalCopies,
            AvailableCopies = b.AvailableCopies,
            CoverImageUrl = b.CoverImageUrl,
            CreatedAt = b.CreatedAt
        };

        private IEnumerable<SelectListItem> GetAuthorSelectList(int selectedId = 0)
            => _uow.Authors.GetAll()
                           .Select(a => new SelectListItem
                           {
                               Value = a.Id.ToString(),
                               Text = a.FullName,
                               Selected = a.Id == selectedId
                           });

        private IEnumerable<SelectListItem> GetCategorySelectList(int selectedId = 0)
            => _uow.Categories.GetAll()
                              .Where(c => !c.IsDeleted)
                              .Select(c => new SelectListItem
                              {
                                  Value = c.Id.ToString(),
                                  Text = c.Name,
                                  Selected = c.Id == selectedId
                              });

        // ── Image Handler
        private static string? SaveImage(IFormFile? file, string wwwRootPath, string? oldImagePath = null)
        {
           
            if (file == null || file.Length == 0)
                return oldImagePath;

          
            var folderPath = Path.Combine(wwwRootPath, "images", "books");

        
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

           
            if (!string.IsNullOrEmpty(oldImagePath))
            {
                var oldFilePath = Path.Combine(wwwRootPath, oldImagePath.TrimStart('/'));
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

           
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(folderPath, fileName);

            
            using (var stream = new FileStream(fullPath, FileMode.Create))
                file.CopyTo(stream);

          
            return $"/images/books/{fileName}";
        }

        // ── Queries ───────────────────────────────────────────────────────────

        public IEnumerable<BookViewModel> GetAll()
            => _uow.Books.GetAllWithDetails().Select(ToViewModel);

        public BookViewModel? GetById(int id)
        {
            var book = _uow.Books.GetWithDetails(id);
            return book is null ? null : ToViewModel(book);
        }

        public BookFormViewModel GetCreateForm()
            => new BookFormViewModel
            {
                Authors = GetAuthorSelectList(),
                Categories = GetCategorySelectList()
            };

        public BookFormViewModel? GetEditForm(int id)
        {
            var book = _uow.Books.GetById(id);
            if (book is null) return null;

            return new BookFormViewModel
            {
                Id = book.Id,
                ISBN = book.ISBN,
                Title = book.Title,
                Description = book.Description,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                PublishedYear = book.PublishedYear,
                BorrowFee = book.BorrowFee,
                DailyFineRate = book.DailyFineRate,
                TotalCopies = book.TotalCopies,
                CoverImageUrl = book.CoverImageUrl,   
                Authors = GetAuthorSelectList(book.AuthorId),
                Categories = GetCategorySelectList(book.CategoryId)
            };
        }

        // ── Commands ──────────────────────────────────────────────────────────

        public BookViewModel Create(BookFormViewModel vm, string wwwRootPath)
        {
            var imagePath = SaveImage(vm.CoverImage, wwwRootPath);

            var book = new Book
            {
                ISBN = vm.ISBN,
                Title = vm.Title,
                Description = vm.Description,
                AuthorId = vm.AuthorId,
                CategoryId = vm.CategoryId,
                PublishedYear = vm.PublishedYear,
                BorrowFee = vm.BorrowFee,
                DailyFineRate = vm.DailyFineRate,
                TotalCopies = vm.TotalCopies,
                AvailableCopies = vm.TotalCopies,
                CoverImageUrl = imagePath         
            };

            _uow.Books.Add(book);
            _uow.Save();
            return ToViewModel(book);
        }

        public BookViewModel? Update(BookFormViewModel vm, string wwwRootPath)
        {
            var book = _uow.Books.GetById(vm.Id);
            if (book is null) return null;

            int borrowedCopies = book.TotalCopies - book.AvailableCopies;

            if (vm.TotalCopies < borrowedCopies)
                throw new InvalidOperationException(
                    "Total copies cannot be less than the number of currently borrowed copies."
                );

          
            book.CoverImageUrl = SaveImage(vm.CoverImage, wwwRootPath, book.CoverImageUrl);

            book.ISBN = vm.ISBN;
            book.Title = vm.Title;
            book.Description = vm.Description;
            book.AuthorId = vm.AuthorId;
            book.CategoryId = vm.CategoryId;
            book.PublishedYear = vm.PublishedYear;
            book.BorrowFee = vm.BorrowFee;
            book.DailyFineRate = vm.DailyFineRate;
            book.TotalCopies = vm.TotalCopies;
            book.AvailableCopies = vm.TotalCopies - borrowedCopies;

            _uow.Books.Update(book);
            _uow.Save();
            return ToViewModel(book);
        }

        public bool Delete(int id)
        {
            var book = _uow.Books.GetById(id);
            if (book is null) return false;

            // delete image file from disk when book is deleted
            // (wwwRootPath not available here — handled in controller)
            _uow.Books.Delete(id);
            _uow.Save();
            return true;
        }
    }
}
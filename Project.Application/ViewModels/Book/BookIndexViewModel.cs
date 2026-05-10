using System;
using System.Collections.Generic;
using System.Text;


namespace Project.Application.ViewModels.Book
{
    public class BookIndexViewModel
    {
        public IEnumerable<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public string? SearchTerm { get; set; }
        public int? FilterCategoryId { get; set; }

        public int TotalBooks => Books.Count();
        public int AvailableBooks => Books.Count(b => b.IsAvailable);

        public IEnumerable<BookViewModel> FilteredBooks =>
            string.IsNullOrWhiteSpace(SearchTerm)
                ? Books
                : Books.Where(b =>
                    b.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.AuthorName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.ISBN.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
    }
}

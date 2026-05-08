using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels
{
 public class CategoryIndexViewModel
    {
        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public string SearchTerm { get; set; }
        public int TotalCategories => Categories.Count();

        public int TotalBooks => Categories.Sum(c => c.BookCount);


        public IEnumerable<CategoryViewModel> FilteredCategories =>
            string.IsNullOrWhiteSpace(SearchTerm)
                ? Categories
                : Categories.Where(c => c.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
    }
}


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Project.Application.ViewModels.Book
{
    public class BookFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(20, ErrorMessage = "ISBN must not exceed 20 characters.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be 1-200 characters.")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [Range(1000, 2100, ErrorMessage = "Enter a valid year.")]
        public int? PublishedYear { get; set; }

        [Required]
        [Range(0, 9999, ErrorMessage = "Borrow fee must be a positive value.")]
        public decimal BorrowFee { get; set; }

        [Required]
        [Range(0, 9999, ErrorMessage = "Daily fine rate must be a positive value.")]
        public decimal DailyFineRate { get; set; }

        [Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1.")]
        public int TotalCopies { get; set; }

        public string? CoverImageUrl { get; set; }
        [Display(Name = "Cover Image")]
        public IFormFile? CoverImage { get; set; }


        // Dropdowns — populated by the service before sending to view
        public IEnumerable<SelectListItem> Authors { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        // Helpers
        public bool IsEditMode => Id > 0;
        public string FormTitle => IsEditMode ? "Edit Book" : "Add New Book";
    }
}
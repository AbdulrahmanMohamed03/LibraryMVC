using System;
using System.Collections.Generic;
using System.Text;


namespace Project.Application.ViewModels.Book
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string AuthorName { get; set; }
        public int AuthorId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public int? PublishedYear { get; set; }
        public decimal BorrowFee { get; set; }
        public decimal DailyFineRate { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CreatedAtDisplay => CreatedAt.ToString("MMM dd, yyyy");
        public bool IsAvailable => AvailableCopies > 0;
    }
}
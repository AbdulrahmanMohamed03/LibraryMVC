using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int? PublishedYear { get; set; }
        public decimal BorrowFee { get; set; }
        public decimal DailyFineRate { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string? CoverImageUrl { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<BorrowingRecord> BorrowingRecords { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}

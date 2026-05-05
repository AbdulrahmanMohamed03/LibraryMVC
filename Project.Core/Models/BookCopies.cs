using Project.Core.Enums.Books;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class BookCopies
    {
        public int Id { get; set; }
        public string? ShelfLocation { get; set; }
        public BookStatus Status { get; set; }
        public BookType Type { get; set; }
        public byte? ConditionScore { get; set; }

        public decimal? ReplacementCost { get; set; }
        public string? DownloadUrl { get; set; }
        public string? DrmToken { get; set; }
        public DateTime? AccessExpiry { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        public ICollection<BorrowTransaction> BorrowTransactions { get; set; }

    }
}

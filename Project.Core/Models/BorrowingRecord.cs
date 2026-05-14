using Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class BorrowingRecord
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }
        [ForeignKey("BorrowingFeeTransaction")]
        public int? BorrowingFeeTransactionId { get; set; }
        public Transaction? BorrowingFeeTransaction { get; set; }
        [ForeignKey("FineTransaction")]
        public int? FineTransactionId { get; set; }
        public Transaction? FineTransaction { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CheckedOutAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public BorrowingStatus Status { get; set; }
        public decimal AccruedFine { get; set; }
        [ForeignKey("ProcessedByLibrarian")]
        public string? ProcessedByLibrarianId { get; set; }
        public ApplicationUser? ProcessedByLibrarian { get; set; }
        public string? Notes { get; set; }
    }
}

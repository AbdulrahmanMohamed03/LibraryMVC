using Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.BorrowingRecord
{
    
    public class BorrowingDetailsVM
    {
        public int Id { get; set; }
        public string BorrowerName { get; set; }
        public string  NationalId { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        public string Category { get; set; }
        public BorrowingStatus Status { get; set; }
        public string StatusLabel => Status.ToString();
        public DateTime RequestedAt { get; set; }
        public DateTime? CheckedOutAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public decimal BorrowingFeeAmount { get; set; }
        public decimal FineAmount { get; set; }
        public decimal TotalPaid => BorrowingFeeAmount + FineAmount;
        public string ProcessedByLibrarianName { get; set; }
        public string Notes { get; set; }
    }
}

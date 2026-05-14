using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.BorrowingRecord
{
    public class ReturnBorrowVM
    {
        public int BorrowingRecordId { get; set; }
        public string BorrowerName { get; set; }
        public string BookTitle { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public decimal BorrowFee { get; set; }
        public decimal FineAmount { get; set; }
        public int LateDays { get; set; }
        public decimal TotalAmount => BorrowFee + FineAmount;
    }
}

using Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.BorrowingRecord
{
    public class BorrowingRecordVM
    {
        public int Id { get; set; }
        public string BorrowerName { get; set; }
        public string BookTitle { get; set; }
        public BorrowingStatus Status { get; set; }
        public string StatusLabel => Status switch
        {
            BorrowingStatus.Pending => "Pending",
            BorrowingStatus.Active => "Borrowed",
            BorrowingStatus.ReturnedOverdue => "Returned — Overdue",
            BorrowingStatus.Returned => "Returned",
            BorrowingStatus.ReturnedDamaged => "Returned — Damaged",
            _ => Status.ToString()
        };

        public DateTime RequestedAt { get; set; }
        public DateTime? CheckedOutAt { get; set; }
        public DateTime? DueDate { get; set; }

    }
}

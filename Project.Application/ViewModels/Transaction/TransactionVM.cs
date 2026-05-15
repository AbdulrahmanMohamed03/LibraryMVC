using Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Application.ViewModels.Transaction
{
    public class TransactionVM
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string ?LibrarianName { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public TransactionType Type { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

    }
}

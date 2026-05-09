using Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [ForeignKey("Librarian")]
        public string LibrarianId { get; set; }
        public ApplicationUser Librarian { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}

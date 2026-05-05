using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class BorrowTransaction
    {
        public int Id { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueDate { get; set; }
        [ForeignKey("BookCopy")]
        public int BookCopyId { get; set; }
        public BookCopies BookCopy { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Fine> Fines { get; set; }
        public ICollection<TransactionEvents> TransactionEvents { get; set; }
    }
}

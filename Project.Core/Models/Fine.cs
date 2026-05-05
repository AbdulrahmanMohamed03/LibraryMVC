using Project.Core.Enums.Fines;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class Fine
    {
        public int Id { get; set; }
        public FineType FineType { get; set; }
        public int DaysOverDue { get; set; }
        public decimal DailyRate { get; set; }
        public decimal TotalFine { get; set; }
        public FineStatus FineStatus { get; set; }

        [ForeignKey("BorrowTransaction")]
        public int TransactionId { get; set; }
        public BorrowTransaction BorrowTransaction { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}

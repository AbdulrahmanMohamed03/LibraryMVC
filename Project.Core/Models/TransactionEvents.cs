using Project.Core.Enums.TransactionEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class TransactionEvents
    {
        public int Id { get; set; }
        public TransactionEventsStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ActorId { get; set; }
        public string? Metadata { get; set; }
        [ForeignKey("BorrowTransaction")]
        public int TransactionId { get; set; }
        public BorrowTransaction BorrowTransaction { get; set; }
    }
}

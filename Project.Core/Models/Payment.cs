using Project.Core.Enums.Payment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Transactions;

namespace Project.Core.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public PaymentType Type { get; set; }
        public int ReferenceId { get; set; }
        public DateTime PaidAt { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsSuspended { get; set; }

        public Subscription? Subscription { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; } 
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; }
        public ICollection<Fine> Fines { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}

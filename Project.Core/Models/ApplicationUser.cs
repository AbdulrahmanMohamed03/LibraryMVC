using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<UserSubscription> UserSubscriptions { get; set; }
        public ICollection<BorrowingRecord> BorrowingRecords { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Reservation> Reservations { get; set; }

    }
}

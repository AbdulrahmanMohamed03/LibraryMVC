using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MonthlyBorrowLimit { get; set; }
        public int LoanDurationDays { get; set; }
        public decimal MonthlyFee { get; set; }
        public ICollection<UserSubscription> UserSubscriptions { get; set; }
    }
}

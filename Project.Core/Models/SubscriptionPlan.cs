using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BorrowLimit { get; set; }
        public int LoanDays { get; set; }
        public int MaxRenewals { get; set; }
        public decimal FineRatePerDay { get; set; }
        public int GracePeriodDays { get; set; }
        public bool DigitalAccess { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }

    }
}

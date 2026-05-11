using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.UserSubscription
{
    public class UserSubscriptionVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public decimal MonthlyFee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}

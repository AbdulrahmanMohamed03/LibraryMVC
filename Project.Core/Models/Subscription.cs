using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiresAt { get; set; }

        [ForeignKey("Plan")]
        public int PlanId { get; set; }
        public SubscriptionPlan Plan { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}

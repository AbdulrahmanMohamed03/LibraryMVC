using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.ViewModels.UserSubscription
{
    public class CreateUserSubscriptionVM
    {
        [Required(ErrorMessage ="User is Required")]
        public string UserId {  get; set; }

        [Required(ErrorMessage ="Plan is Required")]
        public int PlanId {  get; set; }
    }
}

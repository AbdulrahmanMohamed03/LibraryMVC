using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.ViewModels.SubscriptionPlan
{
    public class UpdateSupscriptionPlanDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Plan Name is required")]
        public string Name { get; set; }

        [Range(1, 100, ErrorMessage = "Borrow limit must be between 1 and 100 books.")]
        [Display(Name = "Monthly Borrow Limit")]
        public int MonthlyBorrowLimit { get; set; }

        [Display(Name = "Loan Duration (Days)")]
        [Range(1, 365, ErrorMessage = "Loan duration must be between 1 and 365 days.")]
        public int LoanDurationDays { get; set; }

        [Display(Name = "Monthly Fee")]
        [Range(0, 100000, ErrorMessage = "Monthly fee must be a valid positive amount.")]
        [DataType(DataType.Currency)]
        public decimal MonthlyFee { get; set; }
    }
}

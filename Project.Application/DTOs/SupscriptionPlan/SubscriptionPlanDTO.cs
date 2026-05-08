using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.DTOs.SupscriptionPlan
{
    public class SubscriptionPlanDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MonthlyBorrowLimit { get; set; }
        public int LoanDurationDays { get; set; }
        public decimal MonthlyFee { get; set; }


    }
}

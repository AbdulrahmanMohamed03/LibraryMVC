using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Configurations
{
    public class SubscriptionPlanConfigurations : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
        {
            builder.Property(x => x.MonthlyFee)
                   .HasPrecision(18, 2);
            builder.HasData(
                new SubscriptionPlan
                {
                    Id = 1,
                    Name = "Free",
                    MonthlyFee = 0m,
                    LoanDurationDays = 2,
                    MonthlyBorrowLimit = 1
                }
            );
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Configurations
{
    public class BookConfigurations : IEntityTypeConfiguration<Book>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Book> builder)
        {
            builder.HasIndex(x => x.ISBN)
               .IsUnique();
            builder.Property(x => x.BorrowFee)
                .HasColumnType("decimal(10,2)");
            builder.Property(x => x.DailyFineRate)
                .HasColumnType("decimal(10,2)");
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                "CK_Books_AvailableCopies",
                "AvailableCopies >= 0"
                );
                t.HasCheckConstraint(
                "CK_Books_TotalCopies",
                "TotalCopies >= AvailableCopies"
                );
            });
        }
    }
}

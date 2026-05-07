using Microsoft.EntityFrameworkCore;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Configurations
{
    public class TransactionConfigurations : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Transaction> builder)
        {
            builder.Property(x => x.Amount)
                .HasColumnType("decimal(10,2)");
            builder.HasIndex(x => x.RecordedAt);
            builder.HasOne(x => x.User)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Librarian)
                .WithMany()
                .HasForeignKey(x => x.LibrarianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Configurations
{
    public class BorrowingRecordConfigurations : IEntityTypeConfiguration<BorrowingRecord>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BorrowingRecord> builder)
        {
            builder.Property(x => x.AccruedFine)
                .HasColumnType("decimal(10,2)");
            builder.HasOne(x => x.BorrowingFeeTransaction)
                .WithMany()
                .HasForeignKey(x => x.BorrowingFeeTransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.FineTransaction)
                .WithMany()
                .HasForeignKey(x => x.FineTransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(x => new
            {
                x.UserId,
                x.BookId,
                x.Status
            });
            builder.HasOne(x => x.User)
                .WithMany(x => x.BorrowingRecords)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.ProcessedByLibrarian)
                .WithMany()
                .HasForeignKey(x => x.ProcessedByLibrarianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Configurations
{
    public class ReservationConfigurations : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Reservation> builder)
        {
            builder.HasIndex(x => new
            {
                x.BookId,
                x.Status
            });
        }
    }
}

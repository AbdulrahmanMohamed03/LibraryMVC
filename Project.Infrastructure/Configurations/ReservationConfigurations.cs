using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Core.Models;

namespace Project.Infrastructure.Configurations
{
    public class ReservationConfigurations : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
        
            builder.HasIndex(x => new { x.BookId, x.Status });

       
            builder.HasIndex(x => new { x.UserId, x.Status });

        
            builder.HasIndex(x => x.ExpiresAt);


           

          
            builder.HasOne(x => x.User)
                   .WithMany(x => x.Reservations)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

          
            builder.HasOne(x => x.Book)
                   .WithMany(x => x.Reservations)
                   .HasForeignKey(x => x.BookId)
                   .OnDelete(DeleteBehavior.Restrict);


       
            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Reservations_ExpiresAt",
                    "[ExpiresAt] > [ReservedAt]"
                );
            });
        }
    }
}
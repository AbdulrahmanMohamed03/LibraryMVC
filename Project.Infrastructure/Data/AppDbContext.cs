using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; }
        public DbSet<TransactionEvents> TransactionEvents { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopies> BookCopies { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Fine> Fines { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreating(modelBuilder);
                        
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.BookId, r.Position })
                .IsUnique();
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.UserId, r.BookId })
                .IsUnique();
                       
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.Actor)
                .WithMany() // أو .WithMany(u => u.AuditLogs) إذا كان التعريف موجوداً في كلاس User
                .HasForeignKey(a => a.ActorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Fines -> BorrowTransactions (حماية سجلات الغرامات عند حذف عملية الاستعارة)
            modelBuilder.Entity<Fine>()
                .HasOne(f => f.BorrowTransaction)
                .WithMany()
                .HasForeignKey(f => f.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. BorrowTransactions -> BookCopies (حماية تاريخ الاستعارات عند حذف نسخة كتاب)
            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(t => t.BookCopy)
                .WithMany()
                .HasForeignKey(t => t.BookCopyId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Subscriptions -> SubscriptionPlans (منع حذف خطة اشتراك إذا كان هناك مشتركين فيها)
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Plan)
                .WithMany()
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // ملاحظة إضافية من المراجعة: حل مشكلة الـ Shadow Property في TransactionEvents
            modelBuilder.Entity<TransactionEvents>()
                .HasOne(e => e.BorrowTransaction)
                .WithMany()
                .HasForeignKey(e => e.TransactionId) // ربط صريح بالمفتاح الذي أنشأته أنت
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

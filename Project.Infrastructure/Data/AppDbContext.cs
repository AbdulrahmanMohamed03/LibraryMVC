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
    }
}

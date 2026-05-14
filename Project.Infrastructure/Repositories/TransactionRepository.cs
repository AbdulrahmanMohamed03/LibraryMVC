using Project.Core.Enums;
using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext _context) : base(_context)
        {
        }

        public bool UnPaidFinesForUser(string userId)
        {
            return _context.Transactions.Any(t => t.UserId == userId && t.IsPaid == false && (t.Type == TransactionType.Fine || t.Type == TransactionType.Damaged));
        }
    }
}

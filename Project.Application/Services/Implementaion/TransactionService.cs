using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Transaction;
using Project.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Application.Services.Implementaion
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork unit;
        public TransactionService(IUnitOfWork _unit)
        {
            unit = _unit;
        }
        public List<TransactionVM> getall()
        {
            //throw new NotImplementedException();
            var transaction = unit.Transactions.getAllWithInclude();

            var transVM = transaction.Select(s => new TransactionVM
            {
                Id = s.Id,
                UserName = s.User?.UserName,
                LibrarianName = s.Librarian.FullName,
                Amount = s.Amount,
                IsPaid = s.IsPaid,
                Notes = s.Notes,
                PaidAt = s.PaidAt,
                Type = s.Type,
                RecordedAt = s.RecordedAt,
            }).ToList();
            Console.WriteLine(transVM);

            return transVM;

        }
    }
}

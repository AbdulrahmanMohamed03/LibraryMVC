using Project.Application.ViewModels.Transaction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface ITransactionService
    {
        public List<TransactionVM> getall();
    }
}

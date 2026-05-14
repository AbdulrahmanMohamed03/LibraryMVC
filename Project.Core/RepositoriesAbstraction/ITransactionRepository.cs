using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.RepositoriesAbstraction
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        bool UnPaidFinesForUser(string userId);
    }
}

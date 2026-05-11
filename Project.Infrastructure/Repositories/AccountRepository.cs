using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Repositories
{
    public class AccountRepository :  IAccountRepository
    {
        protected readonly AppDbContext _context;
        public AccountRepository( AppDbContext context)
        {
            this._context = context;   
        }
    }
}

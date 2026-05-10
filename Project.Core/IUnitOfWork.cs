using Project.Core.RepositoriesAbstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }

        IAuthorRepository Authors { get; }
        ISubscriptionPlanRepository SubscriptionPlans { get; }

        IAccountRepository Account { get; }
        int Save();
    }
}

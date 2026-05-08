using Project.Core.RepositoriesAbstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        ISubscriptionPlanRepository SubscriptionPlans {  get; }
        int Save();
    }
}

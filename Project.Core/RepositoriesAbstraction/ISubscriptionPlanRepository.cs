using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.RepositoriesAbstraction
{
    public interface ISubscriptionPlanRepository : IBaseRepository<SubscriptionPlan>
    {
        public SubscriptionPlan GetByName(string name);

    }
}

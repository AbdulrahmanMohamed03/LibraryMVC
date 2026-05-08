using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Repositories
{
    public class SubscriptionPlanRepository : BaseRepository<SubscriptionPlan>, ISubscriptionPlanRepository
    {
        public SubscriptionPlanRepository(AppDbContext _context):base(_context)
        {      
        }
        public SubscriptionPlan GetByName(string name)
        {
            return _context.SubscriptionPlans.FirstOrDefault(p => p.Name == name);
        }

    }
}

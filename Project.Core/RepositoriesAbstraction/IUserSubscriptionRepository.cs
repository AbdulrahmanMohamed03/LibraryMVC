using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.RepositoriesAbstraction
{
    public interface IUserSubscriptionRepository : IBaseRepository<UserSubscription>
    {
        Task<UserSubscription?> GetActiveSubscriptionByUserAsync(string userId);
        Task<IEnumerable<UserSubscription>> GetAllWithDetailsAsync();
        Task<UserSubscription?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<UserSubscription>> GetSubscriptionHistoryAsync(string userId);

    }
}

using Project.Application.ViewModels.UserSubscription;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface IUserSubscriptionService
    {
        Task<UserSubscriptionVM> SubscribeAsync(CreateUserSubscriptionVM vm);
        Task<IEnumerable<UserSubscriptionVM>> GetAllAsync();
        Task<UserSubscriptionVM?> GetByIdAsync(int id);
        Task<UserSubscriptionVM?> GetActiveSubscriptionByUserAsync(string userId);
        Task<IEnumerable<UserSubscriptionVM>> GetSubscriptionHistoryAsync(string userId);
        Task<bool> CancelSubscriptionAsync(int id, string userId);
        Task<bool> CanUserSubscribeAsync(string userId);
        Task<bool> RenewSubscriptionAsync(string userId);

    }
}

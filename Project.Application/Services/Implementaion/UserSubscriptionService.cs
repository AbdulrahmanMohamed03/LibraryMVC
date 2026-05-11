using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.UserSubscription;
using Project.Core;
using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Implementaion
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserSubscriptionService(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;

        }


        // ─── Subscribe ───────────────────────────────────────────
        public async Task<UserSubscriptionVM> SubscribeAsync(CreateUserSubscriptionVM vm)
        {
            // Block if user already has an active subscription
            var canSubscribe = await CanUserSubscribeAsync(vm.UserId);
            if (!canSubscribe) return null;

            var plan = _unitOfWork.SubscriptionPlans.GetById(vm.PlanId);
            if (plan == null) return null;

            var subscription = new UserSubscription
            {
                UserId = vm.UserId,
                PlanId = vm.PlanId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(plan.LoanDurationDays),
                IsActive = true
            };

            _unitOfWork.UserSubscriptions.Add(subscription);
            _unitOfWork.Save();

            // Return full details after creation
            var created = await _unitOfWork.UserSubscriptions.GetByIdWithDetailsAsync(subscription.Id);
                                           
            return MapToVM(created);
        }

        // ─── GetAll ──────────────────────────────────────────────
        public async Task<IEnumerable<UserSubscriptionVM>> GetAllAsync()
        {
            var subscriptions = await _unitOfWork.UserSubscriptions
                                                 .GetAllWithDetailsAsync();
            return subscriptions.Select(u => MapToVM(u)).ToList();
        }

        // ─── GetById ─────────────────────────────────────────────
        public async Task<UserSubscriptionVM?> GetByIdAsync(int id)
        {
            var subscription = await _unitOfWork.UserSubscriptions
                                                .GetByIdWithDetailsAsync(id);
            if (subscription == null) return null;
            return MapToVM(subscription);
        }

        // ─── GetActiveSubscriptionByUser ─────────────────────────
        public async Task<UserSubscriptionVM?> GetActiveSubscriptionByUserAsync(string userId)
        {
            var subscription = await _unitOfWork.UserSubscriptions
                                                .GetActiveSubscriptionByUserAsync(userId);
            if (subscription == null) return null;
            return MapToVM(subscription);
        }

        // ─── GetSubscriptionHistory ──────────────────────────────
        public async Task<IEnumerable<UserSubscriptionVM>> GetSubscriptionHistoryAsync(string userId)
        {
            var history = await _unitOfWork.UserSubscriptions
                                           .GetSubscriptionHistoryAsync(userId);
            return history.Select(u => MapToVM(u)).ToList();
        }

        // ─── CancelSubscription ──────────────────────────────────
        public async Task<bool> CancelSubscriptionAsync(int id,string userId)
        {
            var subscription = await _unitOfWork.UserSubscriptions
                                                .GetByIdWithDetailsAsync(id);
            //if (subscription == null) return false;
            if (subscription == null || subscription.UserId != userId || !subscription.IsActive)
                return false;
            subscription.IsActive = false;
            _unitOfWork.UserSubscriptions.Update(subscription);
            _unitOfWork.Save();
            return true;
        }

        // ─── CanUserSubscribe ────────────────────────────────────
        public async Task<bool> CanUserSubscribeAsync(string userId)
        {
            var activeSubscription = await _unitOfWork.UserSubscriptions
                                                      .GetActiveSubscriptionByUserAsync(userId);
            // Can subscribe only if no active subscription exists
            return activeSubscription == null;
        }

        // ─── RenewSubscription ───────────────────────────────────
        public async Task<bool> RenewSubscriptionAsync(string userId)
        {
            var subscription = await _unitOfWork.UserSubscriptions
                                                .GetActiveSubscriptionByUserAsync(userId);
            if (subscription == null) return false;

            var plan = _unitOfWork.SubscriptionPlans.GetById(subscription.PlanId); 
            if (plan == null) return false;

            subscription.StartDate = DateTime.Now;
            subscription.EndDate = DateTime.Now.AddDays(plan.LoanDurationDays);
            subscription.IsActive = true;

            _unitOfWork.UserSubscriptions.Update(subscription);
            _unitOfWork.Save();
            return true;
        }

        // ─── Private Mapper ──────────────────────────────────────
        private UserSubscriptionVM MapToVM(UserSubscription u)
        {
            if (u == null) return new UserSubscriptionVM();
            return new UserSubscriptionVM
            {
                Id = u.Id,
                UserId = u.UserId,
                UserName = u.User?.UserName,
                PlanId = u.PlanId,
                PlanName = u.Plan?.Name,
                MonthlyFee = u.Plan?.MonthlyFee ?? 0,
                StartDate = u.StartDate,
                EndDate = u.EndDate,
                IsActive = u.IsActive
            };
        }
    }
}

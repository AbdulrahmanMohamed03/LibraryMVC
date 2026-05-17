using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.UserSubscription;
using Project.Core;
using Project.Core.Enums;
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
            var plan = _unitOfWork.SubscriptionPlans.GetById(vm.PlanId);
            if (plan == null) return null;

            
            var existingSubscription = await _unitOfWork.UserSubscriptions
                                                        .GetActiveSubscriptionByUserAsync(vm.UserId);
            if (existingSubscription != null)
            {
                existingSubscription.IsActive = false;
                _unitOfWork.UserSubscriptions.Update(existingSubscription);
            }

            var subscription = new UserSubscription
            {
                UserId = vm.UserId,
                PlanId = vm.PlanId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                IsActive = true
            };

            _unitOfWork.UserSubscriptions.Add(subscription);

            if (plan.MonthlyFee > 0)
            {
                var transaction = new Transaction
                {
                    UserId = vm.UserId,
                    LibrarianId = vm.UserId,
                    Amount = plan.MonthlyFee,
                    Type = TransactionType.Subscription,
                    RecordedAt = DateTime.UtcNow,
                    IsPaid = true,
                    PaidAt = DateTime.UtcNow,
                    Notes = $"Subscription to {plan.Name} plan"
                };
                _unitOfWork.Transactions.Add(transaction);
            }
            _unitOfWork.Save();


            
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
            subscription.EndDate = DateTime.Now.AddDays(30);
            subscription.IsActive = true;

            _unitOfWork.UserSubscriptions.Update(subscription);
            if (plan.MonthlyFee > 0)
            {
                var transaction = new Transaction
                {
                    UserId = userId,
                    LibrarianId = userId,
                    Amount = plan.MonthlyFee,
                    Type = TransactionType.Subscription,
                    RecordedAt = DateTime.UtcNow,
                    IsPaid = true,
                    PaidAt = DateTime.UtcNow,
                    Notes = $"Renewal of {plan.Name} plan"
                };
                _unitOfWork.Transactions.Add(transaction);
            }
            _unitOfWork.Save();
            return true;
        }
        // ─── Check and Auto-Downgrade if Expired ─────────────────
        public async Task CheckAndDowngradeIfExpiredAsync(string userId)
        {
            
            var activeSubscription = await _unitOfWork.UserSubscriptions
                                                      .GetActiveSubscriptionByUserAsync(userId);

            if (activeSubscription != null) return;

            
            var expiredSubscription = await _unitOfWork.UserSubscriptions
                                                       .GetExpiredSubscriptionByUserAsync(userId);

            if (expiredSubscription == null) return; 

            
            expiredSubscription.IsActive = false;
            _unitOfWork.UserSubscriptions.Update(expiredSubscription);

            
            var freePlan = _unitOfWork.SubscriptionPlans.GetByName("Free");
            if (freePlan == null) return;

            var freeSubscription = new UserSubscription
            {
                UserId = userId,
                PlanId = freePlan.Id,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                IsActive = true
            };

            _unitOfWork.UserSubscriptions.Add(freeSubscription);
            _unitOfWork.Save();
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

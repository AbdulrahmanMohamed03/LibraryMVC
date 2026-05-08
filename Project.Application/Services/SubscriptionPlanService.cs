using Project.Application.DTOs.SupscriptionPlan;
using Project.Application.Interfaces;
using Project.Core;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubscriptionPlanService(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;

        }
        public bool CreateSubscriptionPlan(CreateSubscriptionPlanDTO subscriptionPlanDto)
        {
            var foundSubscriptionPlan = _unitOfWork.SubscriptionPlans.GetByName(subscriptionPlanDto.Name);

            if (foundSubscriptionPlan != null)
                return false;

            var subscriptionPlan = new SubscriptionPlan
            {
                Name = subscriptionPlanDto.Name,
                MonthlyFee = subscriptionPlanDto.MonthlyFee,
                MonthlyBorrowLimit= subscriptionPlanDto.MonthlyBorrowLimit,
                LoanDurationDays= subscriptionPlanDto.LoanDurationDays,
            };

            _unitOfWork.SubscriptionPlans.Add(subscriptionPlan);
            _unitOfWork.Save();

            return true;
        }

        public bool UpdateSubscriptionPlan(UpdateSupscriptionPlanDTO subscriptionPlanDto)
        {
            var existingPlan = _unitOfWork.SubscriptionPlans.GetByName(subscriptionPlanDto.Name);

            if (existingPlan != null && existingPlan.Id != subscriptionPlanDto.Id)
                return false;

            var plan = _unitOfWork.SubscriptionPlans.GetById(subscriptionPlanDto.Id);
            if (plan == null) return false;

            plan.Name = subscriptionPlanDto.Name;
            plan.MonthlyBorrowLimit = subscriptionPlanDto.MonthlyBorrowLimit;
            plan.LoanDurationDays = subscriptionPlanDto.LoanDurationDays;
            plan.MonthlyFee = subscriptionPlanDto.MonthlyFee;

            _unitOfWork.SubscriptionPlans.Update(plan);
            _unitOfWork.Save();

            return true;

        }

        public bool DeleteSubscriptionPlan(int id)
        {
            var plan = _unitOfWork.SubscriptionPlans.GetById(id);

            if (plan == null) return false;

            _unitOfWork.SubscriptionPlans.Delete(id);
            _unitOfWork.Save();
            return true;
        }

        



        public IEnumerable<SubscriptionPlanDTO> GetAllSubscriptionPlans()
        {
            var subscriptionPlans = _unitOfWork.SubscriptionPlans.GetAll();

            return subscriptionPlans.Select(p => new SubscriptionPlanDTO
            {
             Id = p.Id,
             Name = p.Name,
             MonthlyBorrowLimit=p.MonthlyBorrowLimit,
             LoanDurationDays= p.LoanDurationDays,
             MonthlyFee= p.MonthlyFee
            }).ToList();
        }

        public UpdateSupscriptionPlanDTO GetSubscriptionPlanById(int id)
        {
            var subscriptionPlan = _unitOfWork.SubscriptionPlans.GetById(id);
            if (subscriptionPlan == null) return null;
            return new UpdateSupscriptionPlanDTO
            {
                Id= subscriptionPlan.Id,
                Name= subscriptionPlan.Name,
                MonthlyFee = subscriptionPlan.MonthlyFee,
                MonthlyBorrowLimit = subscriptionPlan.MonthlyBorrowLimit,
                LoanDurationDays= subscriptionPlan.LoanDurationDays
            };

        }


    }
}

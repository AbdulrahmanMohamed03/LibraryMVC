using Project.Application.ViewModels.SubscriptionPlan;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface ISubscriptionPlanService
    {
        bool CreateSubscriptionPlan(CreateSubscriptionPlanDTO subscriptionPlanDto);
        IEnumerable<SubscriptionPlanDTO> GetAllSubscriptionPlans();

        UpdateSupscriptionPlanDTO GetSubscriptionPlanById(int id);
        bool UpdateSubscriptionPlan(UpdateSupscriptionPlanDTO subscriptionPlanDto);
        bool DeleteSubscriptionPlan(int id);

    }
}

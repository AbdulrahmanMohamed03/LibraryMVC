using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Account;
using Project.Application.ViewModels.SubscriptionPlan;
using Project.Application.ViewModels.UserSubscription;
using Project.Core;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Application.Services.Implementaion
{
    public class AccountService : IAccountService
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IUserSubscriptionService userSub;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUnitOfWork unitOfWork, IUserSubscriptionService userSub)
        {
            _userManager = userManager;
            this.signInManager = signInManager;
            this._unitOfWork = unitOfWork;
            this.userSub = userSub;
            //this.subscriptionPlan = subscriptionPlan;
        }

        public async Task<SignInResult> Login(LoginVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return SignInResult.Failed;

            var result = await signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.rememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                await userSub.CheckAndDowngradeIfExpiredAsync(user.Id);
            }

            return result;
        }

        public async Task<ServiceResult> register(RegisterVM model)
        {
            try
            {

                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    NationalId = model.NationalId,
                    Email = model.Email,
                    UserName = model.Email,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Errors = result.Errors
                                       .Select(e => e.Description)
                                       .ToList()
                    };
                }
                await _userManager.AddToRoleAsync(user, "User");
                await signInManager.SignInAsync(user, false);


                var plane = _unitOfWork.SubscriptionPlans.GetByName("Free");

                var registerUser = new CreateUserSubscriptionVM
                {
                    PlanId= plane.Id,
                    UserId = user.Id,
                };
                await userSub.SubscribeAsync(registerUser);


                return new ServiceResult
                {
                    Success = true,
                    Message = "Account created successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        ex.Message
                    }
                };
            }
        }

        public async Task Logout()
        {
            await signInManager.SignOutAsync();
        }

    }
}

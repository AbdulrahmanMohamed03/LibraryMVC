using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Account;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Implementaion
{
    public class AccountService : IAccountService
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<SignInResult> Login(LoginVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return SignInResult.Failed;

            return await signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.rememberMe,
                lockoutOnFailure: false
            );
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
                await signInManager.SignInAsync(user, false);

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

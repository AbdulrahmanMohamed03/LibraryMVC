using Microsoft.AspNetCore.Identity;
using Project.Application.ViewModels.Account;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResult> register(RegisterVM model);
        Task<SignInResult> Login(LoginVM model);

        Task<ApplicationUser> GetUserByEmail(string email);
        Task<bool> IsUserInRoleAsync(ApplicationUser user, string role);

        Task Logout();
    }
}

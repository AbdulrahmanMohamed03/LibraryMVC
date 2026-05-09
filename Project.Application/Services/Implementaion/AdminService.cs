using Microsoft.AspNetCore.Identity;
using Project.Application.Services.Interfaces;
using Project.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Project.Application.ViewModels.Role;
using Project.Application.ViewModels.User;

namespace Project.Application.Services.Implementaion
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            this._userManager = _userManager;
            this._roleManager = _roleManager;
        }
        public async Task<AssignRoleVm> AssignRoleToUserAsync(AssignRoleVm vm)
        {
            vm.IsSuccess = false;
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                vm.Message = "User not found.";
                return vm;
            }

            var roleExists = await _roleManager.RoleExistsAsync(vm.RoleName);
            if (!roleExists)
            {
                vm.Message = "Role not found.";
                return vm;
            }

            var isInRole = await _userManager.IsInRoleAsync(user, vm.RoleName);
            if (isInRole)
            {
                vm.Message = "User already has this role.";
                return vm;
            }

            var result = await _userManager.AddToRoleAsync(user, vm.RoleName);
            if (!result.Succeeded)
            {
                vm.Message = string.Join(", ", result.Errors.Select(e => e.Description));
                return vm;
            }

            vm.IsSuccess = true;
            vm.Message = "Role assigned successfully.";
            return vm;
        }

        public async Task<CreateLibrarianVM> CreateLibrarianAsync(CreateLibrarianVM vm)
        {
            vm.IsSuccess = false;
            if (string.IsNullOrEmpty(vm.RoleName))
            {
                vm.Message = "Role is required.";
                return vm;
            }

            var roleExists = await _roleManager.RoleExistsAsync(vm.RoleName);
            if (!roleExists)
            {
                vm.Message = "Invalid role selected.";
                return vm;
            }

            var existingUser = await _userManager.FindByEmailAsync(vm.Email);
            if (existingUser != null)
            {
                vm.Message = "User with this email already exists.";
                return vm;
            }

            var existingUserName = await _userManager.FindByNameAsync(vm.UserName);
            if (existingUserName != null)
            {
                vm.Message = "User with this username already exists.";
                return vm;
            }

            var user = new ApplicationUser
            {
                FullName = vm.FullName,
                NationalId = vm.NationalId,
                Email = vm.Email,
                UserName = vm.UserName,
                IsActive = vm.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, vm.Password);

            if (!createResult.Succeeded)
            {
                vm.Message = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return vm;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, vm.RoleName);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                vm.Message = "User was created but role assignment failed. Operation rolled back.";
                return vm;
            }
            vm.IsSuccess = true;
            vm.Message = "Librarian created successfully.";

            return vm;
        }

        public async Task<List<RoleVM>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => new RoleVM { Id = r.Id, Name = r.Name }).ToList();
        }

        //public async Task<List<AssignRoleVm>> GetUserRolesAsync(string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        return new List<AssignRoleVm>();
        //    }

        //    var userRoles = await _userManager.GetRolesAsync(user);
        //    return userRoles.Select(role => new AssignRoleVm
        //    {
        //        Email = email,
        //        RoleName = role
        //    }).ToList();
        //}

        //public async Task<AssignRoleVm> RemoveUserFromRoleAsync(AssignRoleVm vm)
        //{
        //    vm.IsSuccess = false;
        //    var user = await _userManager.FindByEmailAsync(vm.Email);
        //    if (user == null)
        //    {
        //        vm.Message = "User not found.";
        //        return vm;
        //    }

        //    var roleExists = await _roleManager.RoleExistsAsync(vm.RoleName);
        //    if (!roleExists)
        //    {
        //        vm.Message = "Role not found.";
        //        return vm;
        //    }

        //    var isInRole = await _userManager.IsInRoleAsync(user, vm.RoleName);
        //    if (!isInRole)
        //    {
        //        vm.Message = "User does not have this role.";
        //        return vm;
        //    }

        //    var result = await _userManager.RemoveFromRoleAsync(user, vm.RoleName);
        //    if (!result.Succeeded)
        //    {
        //        vm.Message = string.Join(", ", result.Errors.Select(e => e.Description));
        //        return vm;
        //    }

        //    vm.IsSuccess = true;
        //    vm.Message = "Role removed successfully.";
        //    return vm;
        //}
    }
}

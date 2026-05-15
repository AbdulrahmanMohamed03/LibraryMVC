using Microsoft.AspNetCore.Identity;
using Project.Application.Services.Interfaces;
using Project.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Project.Application.ViewModels.Role;
using Project.Application.ViewModels.User;
using Project.Application.ViewModels.Admin;
using Project.Core;

namespace Project.Application.Services.Implementaion
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public AdminService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, IUnitOfWork _unitOfWork)
        {
            this._userManager = _userManager;
            this._roleManager = _roleManager;
            this._unitOfWork = _unitOfWork;
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

        public async Task<bool> EditLibrarianAsync(EditLibrarianVM vm)
        {
            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null)
                return false;

            user.FullName = vm.FullName;
            user.Email = vm.Email;
            user.UserName = vm.UserName;
            var result = await _userManager.UpdateAsync(user);

            vm.IsSuccess = result.Succeeded;
            vm.Message = result.Succeeded ? "Updated successfully" : "Update failed";
            return result.Succeeded;
        }

        public async Task<bool> FireLibrarianAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return false;

            user.IsActive = false;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return false;

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Librarian"))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, "Librarian");
                if (!removeResult.Succeeded)
                    return false;
            }
            return true;
        }

        public async Task<List<LibrariansDataVM>> GetAllLibrariansAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("Librarian");
            var librarians = users.Select(u => new LibrariansDataVM
            {
                UserId = u.Id,
                FullName = u.FullName,
                NationalId = u.NationalId,
                Email = u.Email,
                HiredAt = u.CreatedAt,
                IsStillWorking = u.IsActive,
                RoleName = "Librarian"
            }).ToList();

            return librarians;
        }

        public async Task<List<LibrariansDataVM>> GetFiredLibrariansAsync()
        {
            var users = await _userManager.Users.Where(u => !u.IsActive).ToListAsync();
            return users.Select(u => new LibrariansDataVM
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    NationalId = u.NationalId,
                    Email = u.Email,
                    HiredAt = u.CreatedAt,
                    IsStillWorking = false,
                    RoleName = "Fired"
                }).ToList();
        }

        public async Task<List<RoleVM>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Where(r => r.NormalizedName != "USER").Select(r => new RoleVM { Id = r.Id, Name = r.Name }).ToList();
        }

        public async Task<AdminDashboardVM> GetDashboardDataAsync()
        {
            int categoriesCount = _unitOfWork.Categories.GetAll().Count();
            int booksCount = _unitOfWork.Books.GetAll().Count();
            int reservationsCount = _unitOfWork.Reservations.GetAll().Count();
            int librariansCount = (await _userManager.GetUsersInRoleAsync("Librarian")).Count(u => u.IsActive);
            var dashboardData = new AdminDashboardVM
            {
                CategoriesCount = categoriesCount,
                BooksCount = booksCount,
                LibrariansCount = librariansCount,
                ReservationsCount = reservationsCount
            };

            return dashboardData;
        }

        public async Task<LibrariansDataVM> GetLibrarianDetailsAsync(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) { 
                return new LibrariansDataVM { IsSuccess = false, Message = "Librarian not found." };
            }

            return new LibrariansDataVM
            {
                UserId = user.Id,
                FullName = user.FullName,
                NationalId = user.NationalId,
                Email = user.Email,
                HiredAt = user.CreatedAt,
                IsStillWorking = user.IsActive,
                RoleName = "Librarian",
                IsSuccess = true
            };
        }

        public async Task<EditLibrarianVM> GetLibrarianForEditAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new EditLibrarianVM { IsSuccess = false, Message = "Librarian not found." };

            return new EditLibrarianVM
            {
                UserId = user.Id,
                FullName = user.FullName,
                NationalId = user.NationalId,
                Email = user.Email,
                UserName = user.UserName,
                IsSuccess = true
            };
        }

        public async Task<List<UserRoleVm>> GetUserRolesAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new List<UserRoleVm>();

            var roles = await _userManager.GetRolesAsync(user);

            return roles.Select(r => new UserRoleVm
            {
                RoleName = r
            }).ToList();
        }

        public async Task<bool> ReHireLibrarianAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return false;

            user.IsActive = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return false;

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Librarian"))
            {
                var addResult = await _userManager.AddToRoleAsync(user, "Librarian");
                if (!addResult.Succeeded)
                    return false;
            }
            return true;
        }
        public async Task<AssignRoleVm> RemoveUserFromRoleAsync(AssignRoleVm vm)
        {
            vm.IsSuccess = false;
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                vm.Message = "User not found.";
                return vm;
            }
            var result = await _userManager.RemoveFromRoleAsync(user, vm.RoleName);
            if (!result.Succeeded)
            {
                vm.Message = string.Join(", ", result.Errors.Select(e => e.Description));
                return vm;
            }
            vm.IsSuccess = true;
            vm.Message = "Role removed successfully.";
            return vm;
        }

    }
}

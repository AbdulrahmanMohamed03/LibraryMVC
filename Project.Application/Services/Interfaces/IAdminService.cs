using Microsoft.AspNetCore.Identity;
using Project.Application.ViewModels.Role;
using Project.Application.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<RoleVM>> GetAllRolesAsync();
        Task<CreateLibrarianVM> CreateLibrarianAsync(CreateLibrarianVM vm);
        Task<AssignRoleVm> AssignRoleToUserAsync(AssignRoleVm vm);
        //Task<AssignRoleVm> RemoveUserFromRoleAsync(AssignRoleVm vm);
        //Task<List<AssignRoleVm>> GetUserRolesAsync(string email);
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.ViewModels.Role
{
    public class ManageUserRolesVm
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public List<UserRoleVm> Roles { get; set; } = new();
        [Required(ErrorMessage = "Please select a role")]
        public string SelectedRole { get; set; }
        public List<SelectListItem> AllRoles { get; set; } = new();
    }
}

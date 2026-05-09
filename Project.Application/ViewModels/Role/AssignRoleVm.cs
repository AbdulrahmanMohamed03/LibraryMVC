using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.ViewModels.Role
{
    public class AssignRoleVm
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Role is required")]
        [StringLength(100)]
        public string RoleName { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.ViewModels.Account
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, ErrorMessage = "Full Name must be between 2 and 100 characters", MinimumLength = 2)]
        public string FullName { get; set; }
        [Required(ErrorMessage = "National ID is required")]
        public string NationalId { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare(otherProperty: "Password",ErrorMessage ="not matched")]
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }



    }
}

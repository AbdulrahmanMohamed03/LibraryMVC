using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.ViewModels.User
{
    public class EditLibrarianVM
    {
        public string UserId { get; set; }

        [Required]
        public string FullName { get; set; }
        public string NationalId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}

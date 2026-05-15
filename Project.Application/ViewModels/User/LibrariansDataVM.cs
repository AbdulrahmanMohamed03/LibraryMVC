using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.ViewModels.User
{
    public class LibrariansDataVM
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public string Email { get; set; }
        public DateTime HiredAt { get; set; }
        public bool IsStillWorking { get; set; }
        public string RoleName { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}

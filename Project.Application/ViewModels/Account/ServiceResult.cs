using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.Account
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.ViewModels
{
    public class CategoryViewModel
    {
        //[Required(ErrorMessage = "Category name is required")]
        //public string Name { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public int BookCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedAtDisplay => CreatedAt.ToString("MMM dd, yyyy");
    }
}

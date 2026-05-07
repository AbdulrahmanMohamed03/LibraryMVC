using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.DTOs
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }
    }
}

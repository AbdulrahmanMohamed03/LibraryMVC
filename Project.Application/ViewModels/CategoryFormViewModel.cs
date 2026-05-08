using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Project.Application.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters.")]
        [Remote(action: "IsNameAvailable", controller: "Category", AdditionalFields = nameof(Id), ErrorMessage = "This name is already taken.")]
        public string Name { get; set; }

        
        public bool IsEditMode => Id > 0;
        public string FormTitle => IsEditMode ? "Edit Category" : "Add New Category";
    }
}

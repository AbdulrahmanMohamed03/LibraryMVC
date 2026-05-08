using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Application.DTOs.Author
{
    public class UpdateAuthorDto
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; }
        public string? Bio { get; set; }
        public string? Nationality { get; set; }
    }
}

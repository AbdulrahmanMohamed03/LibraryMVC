using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.DTOs.Author
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Bio { get; set; }
        public string? Nationality { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
    }
}


using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Bio { get; set; }
        public string? Nationality { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Book> Books { get; set; }
    }
}

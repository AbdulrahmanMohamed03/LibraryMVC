using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Book> Books { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<BookCopies> Copies { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}

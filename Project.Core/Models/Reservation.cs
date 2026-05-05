using Project.Core.Enums.Reservation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public ReservationType Type { get; set; }
        public DateTime ExpiresAt { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}

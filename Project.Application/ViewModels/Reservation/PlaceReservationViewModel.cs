using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.Reservation
{
   public class PlaceReservationViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string BookAuthor { get; set; } = null!;
        public string? BookCoverUrl { get; set; }
        public decimal BorrowFee { get; set; }

      
        public int QueuePosition { get; set; }
    }
}

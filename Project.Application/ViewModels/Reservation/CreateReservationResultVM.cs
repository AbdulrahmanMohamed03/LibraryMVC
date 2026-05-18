using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.Reservation
{
    public class CreateReservationResultVM
    {
        public int? ReservationId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.Reservation
{
    public class UserReservationsViewModel
    {
        //user dashboard view.
        public IEnumerable<ReservationViewModel> Reservations { get; set; } = [];
        public int TotalCount => Reservations.Count();
        public int PendingCount => Reservations.Count(r => r.Status == Core.Enums.ReservationStatus.Pending);
        public int ReadyCount => Reservations.Count(r => r.Status == Core.Enums.ReservationStatus.Ready);
        public int FulfilledCount => Reservations.Count(r => r.Status == Core.Enums.ReservationStatus.Fulfilled);
        public int CancelledCount => Reservations.Count(r => r.Status == Core.Enums.ReservationStatus.Cancelled);

        public IEnumerable<ReservationViewModel> UrgentAlerts =>
            Reservations.Where(r => r.IsExpiringSoon);
    }
}

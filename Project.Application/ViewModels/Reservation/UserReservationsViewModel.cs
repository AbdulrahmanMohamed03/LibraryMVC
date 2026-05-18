using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.Reservation
{
    public class UserReservationsViewModel
    {
        //user dashboard view.
        public IEnumerable<ReservationViewModel> Reservations { get; set; } = [];
        public IEnumerable<ReservationViewModel> AllReservations { get; set; } = [];
        public bool IsAdminView { get; set; } = false;
        private IEnumerable<ReservationViewModel> Active =>
           IsAdminView ? AllReservations : Reservations;

        public int TotalCount => Active.Count();
        public int PendingCount => Active.Count(r => r.Status == Core.Enums.ReservationStatus.Pending);
        public int ReadyCount => Active.Count(r => r.Status == Core.Enums.ReservationStatus.Ready);
        public int FulfilledCount => Active.Count(r => r.Status == Core.Enums.ReservationStatus.Fulfilled);
        public int CancelledCount => Active.Count(r => r.Status == Core.Enums.ReservationStatus.Cancelled);

        public IEnumerable<ReservationViewModel> UrgentAlerts =>
            Reservations.Where(r => r.IsExpiringSoon);
    }
}

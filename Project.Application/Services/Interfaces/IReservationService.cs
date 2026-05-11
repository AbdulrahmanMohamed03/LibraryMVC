using Project.Application.ViewModels.Reservation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface IReservationService
    {

        // ── User-facing ───────────────────────────────────────────────────────
        PlaceReservationViewModel GetPlaceReservationForm(int bookId, string userId);

 
        ReservationViewModel PlaceReservation(int bookId, string userId);

   
        void CancelReservation(int reservationId, string requestingUserId);

    
        UserReservationsViewModel GetUserReservations(string userId);

        // ── System / Librarian-facing ─────────────────────────────────────────

 
        bool CheckAndAssignReservationOnReturn(int bookId);


        int ExpireOverdueReservations();


    }
}

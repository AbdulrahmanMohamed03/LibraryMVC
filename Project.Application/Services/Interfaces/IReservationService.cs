//using Project.Application.ViewModels.Reservation;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Project.Application.Services.Interfaces
//{
//    public interface IReservationService
//    {

//        // ── User-facing ───────────────────────────────────────────────────────
//        PlaceReservationViewModel GetPlaceReservationForm(int bookId, string userId);


//        ReservationViewModel PlaceReservation(int bookId, string userId);


//        void CancelReservation(int reservationId, string requestingUserId);


//        UserReservationsViewModel GetUserReservations(string userId);

//        // ── Admin/Librarian-facing ────────────────────────────────────────────

//        UserReservationsViewModel GetAllReservationsForAdmin();

//        // ── Queue Engine ───────────────────────────────────────────────────────


//        bool CheckAndAssignReservationOnReturn(int bookId);

//        int ProcessNewCopiesIntoQueue(int bookId, int addedCopiesCount);

//        // ── Maintenance ────────────────────────────────────────────────────────
//        int ExpireOverdueReservations();

//        // ── Book details page helper ──────────────────────────────────────────
//        bool UserHasActiveReservationForBook(string userId, int bookId);


//    }
//}


using Project.Application.ViewModels.Reservation;

namespace Project.Application.Services.Interfaces
{
    public interface IReservationService
    {
        PlaceReservationViewModel GetPlaceReservationForm(int bookId, string userId);
        CreateReservationResultVM CheckFormEligibility(int bookId, string userId);
        CreateReservationResultVM PlaceReservation(int bookId, string userId);
        CreateReservationResultVM CancelReservation(int reservationId, string requestingUserId);
        UserReservationsViewModel GetUserReservations(string userId);
        UserReservationsViewModel GetAllReservationsForAdmin();
        bool UserHasActiveReservationForBook(string userId, int bookId);
        bool CheckAndAssignReservationOnReturn(int bookId);
        int ProcessNewCopiesIntoQueue(int bookId, int addedCopiesCount);
        int ExpireOverdueReservations();
        CreateReservationResultVM MarkAsFulfilled(int reservationId);
    }
}
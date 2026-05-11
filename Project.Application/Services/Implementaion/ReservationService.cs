using Microsoft.EntityFrameworkCore;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Reservation;
using Project.Core;
using Project.Core.Enums;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Implementaion
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _uow;

      
        private const int ReadyExpiryDays = 3;

        private const int PendingExpiryDays = 30;

        public ReservationService(IUnitOfWork uow) => _uow = uow;

        // ─────────────────────────────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────────────────────────────

        private static ReservationViewModel ToViewModel(Reservation r) => new()
        {
            Id = r.Id,
            BookId = r.BookId,
            BookTitle = r.Book?.Title ?? "—",
            BookAuthor = r.Book?.Author?.FullName ?? "—",
            BookCoverUrl = r.Book?.CoverImageUrl,
            UserId = r.UserId,
            UserFullName = r.User?.FullName ?? "—",
            UserEmail = r.User?.Email ?? "—",
            ReservedAt = r.ReservedAt,
            ExpiresAt = r.ExpiresAt,
            Status = r.Status
        };


        // Checks all  rules for placing a reservation.
    
        private void ValidateEligibility(string userId, Book book)
        {
            // Rule 1: Book must be fully checked out
            if (book.AvailableCopies > 0)
                throw new InvalidOperationException(
                    $"This book currently has {book.AvailableCopies} available " +
                    "copy(ies). Reservations are only allowed when all copies are checked out.");

            // Rule 2: User must have an active subscription

            //var hasActiveSubscription = _uow.SubscriptionPlans
            //    .GetAll() 
            //    .Any();    


            //var activeSubscription = GetActiveUserSubscription(userId);
            //if (activeSubscription is null)
            //    throw new InvalidOperationException(
            //        "You need an active subscription to place a reservation. " +
            //        "Please subscribe to a plan first.");

            if (!_uow.Reservations.UserHasActiveSubscription(userId))
            {
                throw new InvalidOperationException(
                    "You need an active library subscription to place a reservation. " +
                    "Please subscribe to a plan first.");
            }

            // Rule 3: User must have no unpaid fines (Professional Touch)
            if (_uow.Reservations.UserHasUnpaidFines(userId))
                throw new InvalidOperationException(
                    "You have outstanding borrowing fines that must be paid before " +
                    "you can place a new reservation. Please visit the library desk.");

            // Rule 4: No duplicate active reservation for the same book
            var existing = _uow.Reservations.GetActiveForUserAndBook(userId, book.Id);
            if (existing is not null)
            {
                var statusLabel = existing.Status == ReservationStatus.Ready
                    ? "is already ready for pickup"
                    : "is already in the queue";
                throw new InvalidOperationException(
                    $"You already have an active reservation for '{book.Title}' " +
                    $"that {statusLabel}.");
            }
        }

  
        // ─────────────────────────────────────────────────────────────────────
        // PUBLIC API
        // ─────────────────────────────────────────────────────────────────────

        public PlaceReservationViewModel GetPlaceReservationForm(int bookId, string userId)
        {
            var book = _uow.Books.GetWithDetails(bookId)
                ?? throw new InvalidOperationException("Book not found.");

            ValidateEligibility(userId, book);

            var queuePosition = _uow.Reservations.GetPendingQueueDepth(bookId) + 1;

            return new PlaceReservationViewModel
            {
                BookId = book.Id,
                BookTitle = book.Title,
                BookAuthor = book.Author?.FullName ?? "—",
                BookCoverUrl = book.CoverImageUrl,
                BorrowFee = book.BorrowFee,
                QueuePosition = queuePosition
            };
        }

        public ReservationViewModel PlaceReservation(int bookId, string userId)
        {
            var book = _uow.Books.GetById(bookId)
                ?? throw new InvalidOperationException("Book not found.");

            ValidateEligibility(userId, book);

            // ── Create reservation ────────────────────────────────────────────
            var now = DateTime.UtcNow;
            var reservation = new Reservation
            {
                UserId = userId,
                BookId = bookId,
                ReservedAt = now,
                ExpiresAt = now.AddDays(PendingExpiryDays), 
                Status = ReservationStatus.Pending
            };

            _uow.Reservations.Add(reservation);
            _uow.Save();

           
            var created = _uow.Reservations
                              .GetByUserWithDetails(userId)
                              .First(r => r.Id == reservation.Id);

            return ToViewModel(created);
        }

        public void CancelReservation(int reservationId, string requestingUserId)
        {
            var reservation = _uow.Reservations.GetById(reservationId)
                ?? throw new InvalidOperationException("Reservation not found.");

            // Ownership check 
            if (reservation.UserId != requestingUserId)
                throw new InvalidOperationException(
                    "You do not have permission to cancel this reservation.");

            if (reservation.Status is not (ReservationStatus.Pending or ReservationStatus.Ready))
                throw new InvalidOperationException(
                    $"A reservation with status '{reservation.Status}' cannot be cancelled.");

            var wasReady = reservation.Status == ReservationStatus.Ready;

            reservation.Status = ReservationStatus.Cancelled;
            reservation.ExpiresAt = DateTime.UtcNow; 
            _uow.Reservations.Update(reservation);

            if (wasReady)
            {
               
                // Either give it to the next Pending user, or return it to the pool.
                var bookId = reservation.BookId;
                var promoted = PromoteNextPendingReservation(bookId);

                if (!promoted)
                {
                    
                    var book = _uow.Books.GetById(bookId)!;
                    book.AvailableCopies++;
                    _uow.Books.Update(book);
                }
            }

            _uow.Save();
        }

        public UserReservationsViewModel GetUserReservations(string userId)
        {
            var reservations = _uow.Reservations
                                   .GetByUserWithDetails(userId)
                                   .Select(ToViewModel)
                                   .ToList();

            return new UserReservationsViewModel { Reservations = reservations };
        }

        // ─────────────────────────────────────────────────────────────────────
        // RETURN-TO-RESERVATION BRIDGE
        // Called by BorrowingService after a successful book return.
        // ─────────────────────────────────────────────────────────────────────

        public bool CheckAndAssignReservationOnReturn(int bookId)
        {
            var promoted = PromoteNextPendingReservation(bookId);

            if (!promoted)
            {
                // No pending reservation — release the copy back to the general pool
                var book = _uow.Books.GetById(bookId)!;
                book.AvailableCopies++;
                _uow.Books.Update(book);
                _uow.Save();
            }

            return promoted;
        }

        // ─────────────────────────────────────────────────────────────────────
        // EXPIRY SWEEP
        // Run via a scheduled job (Hangfire / HostedService) or Admin action.
        // ─────────────────────────────────────────────────────────────────────

        public int ExpireOverdueReservations()
        {
            var expired = _uow.Reservations.GetExpired().ToList();
            if (!expired.Any()) return 0;

            // Group by status to handle them differently
            var readyExpired = expired.Where(r => r.Status == ReservationStatus.Ready).ToList();
            var pendingExpired = expired.Where(r => r.Status == ReservationStatus.Pending).ToList();

            // ── Handle Pending expiry: just cancel ────────────────────────────
            foreach (var r in pendingExpired)
            {
                r.Status = ReservationStatus.Cancelled;
                _uow.Reservations.Update(r);
            }

            // ── Handle Ready expiry: cancel + release + try promote ───────────
            foreach (var r in readyExpired)
            {
                r.Status = ReservationStatus.Cancelled;
                _uow.Reservations.Update(r);

                // The copy was held. Try to give it to the next in queue.
                var promoted = PromoteNextPendingReservation(r.BookId);

                if (!promoted)
                {
                    // Nobody waiting — release back to pool
                    var book = _uow.Books.GetById(r.BookId)!;
                    book.AvailableCopies++;
                    _uow.Books.Update(book);
                }
            }

            _uow.Save();
            return expired.Count;
        }

        // ─────────────────────────────────────────────────────────────────────
        // PRIVATE: FIFO promotion
        // ─────────────────────────────────────────────────────────────────────


        private bool PromoteNextPendingReservation(int bookId)
        {
            var next = _uow.Reservations.GetOldestPendingForBook(bookId);
            if (next is null) return false;

            next.Status = ReservationStatus.Ready;
            next.ExpiresAt = DateTime.UtcNow.AddDays(ReadyExpiryDays);
            _uow.Reservations.Update(next);
       

            return true;
        }


    }
}


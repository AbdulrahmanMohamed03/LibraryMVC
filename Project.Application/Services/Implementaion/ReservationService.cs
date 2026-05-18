using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Reservation;
using Project.Core;
using Project.Core.Enums;
using Project.Core.Models;
using System;
using System.Linq;

namespace Project.Application.Services.Implementaion
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _uow;

        private const int ReadyExpiryDays = 3;
        private const int PendingExpiryDays = 30;

        public ReservationService(IUnitOfWork uow) => _uow = uow;

        // ── Mapping ───────────────────────────────────────────────────────────

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

     
        private CreateReservationResultVM ValidateEligibility(string userId, Book book)
        {
            if (book.AvailableCopies > 0)
            {
                return new CreateReservationResultVM
                {
                    IsSuccess = false,
                    Message = $"This book has {book.AvailableCopies} available copy(ies). Reservations are only allowed when all copies are checked out."
                };
            }

            if (!_uow.Reservations.UserHasActiveSubscription(userId))
            {
                return new CreateReservationResultVM
                {
                    IsSuccess = false,
                    Message = "An active library subscription is required to place a reservation."
                };
            }

            if (_uow.Reservations.UserHasUnpaidFines(userId))
            {
                return new CreateReservationResultVM
                {
                    IsSuccess = false,
                    Message = "Outstanding fines must be settled before placing a reservation."
                };
            }

            var existing = _uow.Reservations.GetActiveForUserAndBook(userId, book.Id);
            if (existing is not null)
            {
                var label = existing.Status == ReservationStatus.Ready
                    ? "is already ready for pickup"
                    : "is already in the queue";

                return new CreateReservationResultVM
                {
                    IsSuccess = false,
                    Message = $"You already have an active reservation for '{book.Title}' that {label}."
                };
            }

            return new CreateReservationResultVM { IsSuccess = true };
        }

        // ─────────────────────────────────────────────────────────────────────
        // USER-FACING
        // ─────────────────────────────────────────────────────────────────────

        public PlaceReservationViewModel GetPlaceReservationForm(int bookId, string userId)
        {
            var book = _uow.Books.GetWithDetails(bookId);
            if (book == null) return null!;

            var eligibility = ValidateEligibility(userId, book);
            if (!eligibility.IsSuccess) return null!;

            return new PlaceReservationViewModel
            {
                BookId = book.Id,
                BookTitle = book.Title,
                BookAuthor = book.Author?.FullName ?? "—",
                BookCoverUrl = book.CoverImageUrl,
                BorrowFee = book.BorrowFee,
                QueuePosition = _uow.Reservations.GetPendingQueueDepth(bookId) + 1
            };
        }

        
        public CreateReservationResultVM CheckFormEligibility(int bookId, string userId)
        {
            var book = _uow.Books.GetWithDetails(bookId);
            if (book == null) return new CreateReservationResultVM { IsSuccess = false, Message = "Book not found." };
            return ValidateEligibility(userId, book);
        }

        public CreateReservationResultVM PlaceReservation(int bookId, string userId)
        {
            var book = _uow.Books.GetById(bookId);
            if (book == null)
            {
                return new CreateReservationResultVM { IsSuccess = false, Message = "Book not found." };
            }

            var eligibility = ValidateEligibility(userId, book);
            if (!eligibility.IsSuccess)
            {
                return eligibility;
            }

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

            return new CreateReservationResultVM
            {
                IsSuccess = true,
                ReservationId = reservation.Id,
                Message = book.Title
            };
        }

        public CreateReservationResultVM CancelReservation(int reservationId, string requestingUserId)
        {
            var reservation = _uow.Reservations.GetById(reservationId);
            if (reservation == null)
            {
                return new CreateReservationResultVM { IsSuccess = false, Message = "Reservation not found." };
            }

            if (reservation.UserId != requestingUserId)
            {
                return new CreateReservationResultVM { IsSuccess = false, Message = "You do not have permission to cancel this reservation." };
            }

            if (reservation.Status is not (ReservationStatus.Pending or ReservationStatus.Ready))
            {
                return new CreateReservationResultVM { IsSuccess = false, Message = $"A '{reservation.Status}' reservation cannot be cancelled." };
            }

            bool wasReady = reservation.Status == ReservationStatus.Ready;
            reservation.Status = ReservationStatus.Cancelled;
            reservation.ExpiresAt = DateTime.UtcNow;
            _uow.Reservations.Update(reservation);

            if (wasReady)
            {
                bool promoted = PromoteNextPendingReservation(reservation.BookId);
                if (!promoted)
                {
                    var book = _uow.Books.GetById(reservation.BookId)!;
                    if (book.AvailableCopies < book.TotalCopies)
                    {
                        book.AvailableCopies++;
                    }
                }
            }

            _uow.Save();
            return new CreateReservationResultVM { IsSuccess = true };
        }

        public UserReservationsViewModel GetUserReservations(string userId)
        {
            var vms = _uow.Reservations
                          .GetByUserWithDetails(userId)
                          .Select(ToViewModel)
                          .ToList();
            return new UserReservationsViewModel { Reservations = vms };
        }

        // ── Admin/Librarian ───────────────────────────────────────────────────

        public UserReservationsViewModel GetAllReservationsForAdmin()
        {
            var vms = _uow.Reservations
                          .GetAllWithDetails()
                          .Select(ToViewModel)
                          .ToList();
            return new UserReservationsViewModel
            {
                AllReservations = vms,
                IsAdminView = true
            };
        }

        public bool UserHasActiveReservationForBook(string userId, int bookId)
            => _uow.Reservations.GetActiveForUserAndBook(userId, bookId) is not null;

        // ── Queue bridge ──────────────────────────────────────────────────────

        public bool CheckAndAssignReservationOnReturn(int bookId)
        {
            bool promoted = PromoteNextPendingReservation(bookId);

            if (!promoted)
            {
                var book = _uow.Books.GetById(bookId)!;
                book.AvailableCopies++;
            }

            _uow.Save();
            return promoted;
        }

        public int ProcessNewCopiesIntoQueue(int bookId, int addedCopiesCount)
        {
            if (addedCopiesCount <= 0) return 0;

            var book = _uow.Books.GetById(bookId);
            if (book is null) return 0;

            int promoted = 0;

            for (int i = 0; i < addedCopiesCount; i++)
            {
                if (book.AvailableCopies <= 0) break;

                var next = _uow.Reservations.GetOldestPendingForBook(bookId);
                if (next is null) break;

                next.Status = ReservationStatus.Ready;
                next.ExpiresAt = DateTime.UtcNow.AddDays(ReadyExpiryDays);
                _uow.Reservations.Update(next);

                book.AvailableCopies--;
                promoted++;
            }

            if (promoted > 0)
                _uow.Save();

            return promoted;
        }

        // ─────────────────────────────────────────────────────────────────────
        // MAINTENANCE: Expiry Sweep
        // ─────────────────────────────────────────────────────────────────────

        public int ExpireOverdueReservations()
        {
            var expired = _uow.Reservations.GetExpired().ToList();
            if (!expired.Any()) return 0;

            var readyExpired = expired.Where(r => r.Status == ReservationStatus.Ready).ToList();
            var pendingExpired = expired.Where(r => r.Status == ReservationStatus.Pending).ToList();

            foreach (var r in pendingExpired)
            {
                r.Status = ReservationStatus.Cancelled;
                _uow.Reservations.Update(r);
            }

            foreach (var r in readyExpired)
            {
                r.Status = ReservationStatus.Cancelled;
                _uow.Reservations.Update(r);

                bool promoted = PromoteNextPendingReservation(r.BookId);
                if (!promoted)
                {
                    var book = _uow.Books.GetById(r.BookId)!;
                    if (book.AvailableCopies < book.TotalCopies)
                        book.AvailableCopies++;
                }
            }

            _uow.Save();
            return expired.Count;
        }

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
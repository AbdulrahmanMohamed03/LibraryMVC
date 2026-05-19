using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.BorrowingRecord;
using Project.Core;
using Project.Core.Enums;
using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Project.Application.Services.Implementaion
{
    public class BorrowingService : IBorrowingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationService _reservationService;

    
        public BorrowingService(IUnitOfWork unitOfWork, IReservationService reservationService)
        {
            _unitOfWork = unitOfWork;
            _reservationService = reservationService;
        }

        public bool UserHasActiveBorrowForBook(string userId, int bookId)
        {
          
            return _unitOfWork.BorrowingRecords.ExistingActiveBorrowingForBook(bookId, userId);
        }
        public async Task<CreateBorrowVM> ApproveRequest(int borrowingRecordId, string librarianId)
        {
            var record = _unitOfWork.BorrowingRecords.GetWithDetails(borrowingRecordId);
            if (record == null)
            {
                return new CreateBorrowVM
                {
                    IsSuccess = false,
                    Message = "Borrowing record not found."
                };
            }
            if (record.Status != BorrowingStatus.Pending)
            {
                return new CreateBorrowVM
                {
                    IsSuccess = false,
                    Message = "This request cannot be approved."
                };
            }
            var book = _unitOfWork.Books.GetById(record.BookId);
            if (book == null)
            {
                return new CreateBorrowVM
                {
                    IsSuccess = false,
                    Message = "Book not found."
                };
            }
            if (book.AvailableCopies <= 0)
            {
                return new CreateBorrowVM
                {
                    IsSuccess = false,
                    Message = "No available copies."
                };
            }
            var subscriptionTask = await _unitOfWork.UserSubscriptions
                .GetActiveSubscriptionByUserAsync(record.UserId);

            var subscription = subscriptionTask;
            if (subscription == null)
            {
                return new CreateBorrowVM
                {
                    IsSuccess = false,
                    Message = "No active subscription found. Please subscribe to borrow books."
                };
            }
            var plan = subscription.Plan ?? _unitOfWork.SubscriptionPlans.GetByName("Free");

            var loanDays = plan.LoanDurationDays;

            record.Status = BorrowingStatus.Active;
            record.CheckedOutAt = DateTime.Now;
            record.DueDate = DateTime.Now.AddDays(loanDays);
            record.ProcessedByLibrarianId = librarianId;

            book.AvailableCopies--;

            if (book.AvailableCopies == 0) { 
                var pendingBorrows = _unitOfWork.BorrowingRecords.GetPendingByBookId(record.BookId)
                    .Where(br => br.Id != borrowingRecordId)
                    .ToList();

                foreach (var item in pendingBorrows)
                {
                    var reservation = new Reservation
                    {
                        UserId = item.UserId,
                        BookId = item.BookId,
                        ReservedAt = item.RequestedAt,
                        ExpiresAt = DateTime.Now.AddDays(2),
                        Status = ReservationStatus.Pending
                    };
                    _unitOfWork.Reservations.Add(reservation);
                    _unitOfWork.BorrowingRecords.Delete(item.Id);
                }
            }
            _unitOfWork.Save();
            return new CreateBorrowVM
            {
                IsSuccess = true,
                Message = "Borrow request approved successfully, and all pending requests for books that are now unavailable have been converted to reservations. (if any)",
                BorrowingRecordId = record.Id
            };
        }

        public async Task<CreateBorrowVM> BorrowBook(int bookId, string userId)
        {
            var book = _unitOfWork.Books.GetById(bookId);
            if (book == null) { 
                return new CreateBorrowVM { IsSuccess = false, Message = "Book not found." };
            }
            if(_unitOfWork.BorrowingRecords.ExistingPendingBorrowingForBook(bookId, userId))
            {
                return new CreateBorrowVM { IsSuccess = false, Message = "You already have a pending request for this book\nPlease wait for it to be processed." };
            }
            if(_unitOfWork.BorrowingRecords.ExistingActiveBorrowingForBook(bookId, userId))
            {
                return new CreateBorrowVM { IsSuccess = false, Message = "You already have this book" };
            }
             var subscription = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionByUserAsync(userId);

            if (subscription == null)
            {
                return new CreateBorrowVM
                {
                    IsSuccess = false,
                    Message = "No active subscription found. Please subscribe to borrow books."
                };
            }
            
            var plan = subscription.Plan;
            var startDate = subscription.StartDate;
            var endDate = subscription.EndDate;
            var borrowCount =  _unitOfWork.BorrowingRecords.CountUserBorrowsThisMonth(userId, startDate, endDate);
            if (borrowCount >= plan.MonthlyBorrowLimit)
            {
                return new CreateBorrowVM
                {
                    IsSuccess = false,
                    Message = $"You reached your monthly borrow limit ({plan.MonthlyBorrowLimit})."
                };
            }
            var borrowRequest = new BorrowingRecord
            {
                BookId = bookId,
                UserId = userId,
                Status = BorrowingStatus.Pending,
                RequestedAt = DateTime.Now
            };

            _unitOfWork.BorrowingRecords.Add(borrowRequest);
            _unitOfWork.Save();
            return new CreateBorrowVM { BorrowingRecordId = borrowRequest.Id, IsSuccess = true, Message = "Request submitted successfully. Please wait for it to be processed." };
        }

        public IEnumerable<BorrowingRecordVM> GetAllForLibrarian(string? search = null)
        {
            var records = _unitOfWork.BorrowingRecords.GetAll(search);
            return records.Select(r => new BorrowingRecordVM
            {
                Id = r.Id,
                BorrowerName = r.User.FullName,
                BookTitle = r.Book.Title,
                Status = r.Status,
                RequestedAt = r.RequestedAt,
                CheckedOutAt = r.CheckedOutAt,
                DueDate = r.DueDate
            }).ToList();
        }

        public BorrowingDetailsVM GetDetails(int id)
        {
            var record = _unitOfWork.BorrowingRecords.GetWithDetails(id);
            return new BorrowingDetailsVM
            {
                Id = record.Id,
                BorrowerName = record.User.FullName,
                NationalId = record.User.NationalId,
                BookTitle = record.Book.Title,
                BookAuthor = record.Book.Author?.FullName,
                Category = record.Book.Category?.Name,
                Status = record.Status,
                RequestedAt = record.RequestedAt,
                CheckedOutAt = record.CheckedOutAt,
                DueDate = record.DueDate,
                ReturnedAt = record.ReturnedAt,
                BorrowingFeeAmount = record.BorrowingFeeTransaction?.Amount ?? 0,
                FineAmount = record.FineTransaction?.Amount ?? 0,
                DamageFeeAmount = record.AccruedFine - (record.FineTransaction?.Amount ?? 0),
                ProcessedByLibrarianName = record.ProcessedByLibrarian?.FullName,
                Notes = record.Notes
            };
        }

        public IEnumerable<BorrowingRecordVM> GetPendingRequests(string? search = null)
        {
            var records = _unitOfWork.BorrowingRecords.GetAllPendingForLibrarian(search);

            return records.Select(r => new BorrowingRecordVM
            {
                Id = r.Id,
                BorrowerName = r.User.FullName,
                BookTitle = r.Book.Title,
                Status = r.Status,
                RequestedAt = r.RequestedAt,
                CheckedOutAt = r.CheckedOutAt,
                DueDate = r.DueDate
            }).ToList();
        }

        public ReturnBorrowVM GetReturnDetails(int borrowingRecordId)
        {
            var record = _unitOfWork.BorrowingRecords
                    .GetActiveBorrowing(borrowingRecordId);
            if (record == null)
            {
                return null;
            }
            var returnedAt = DateTime.Now;
            int lateDays = 0;
            if (record.DueDate.HasValue && returnedAt > record.DueDate.Value)
            {
                lateDays = (returnedAt.Date - record.DueDate.Value.Date).Days;
            }
            var fineAmount = lateDays * record.Book.DailyFineRate;
            return new ReturnBorrowVM
            {
                BorrowingRecordId = record.Id,
                BorrowerName = record.User.FullName,
                BookTitle = record.Book.Title,
                DueDate = record.DueDate,
                ReturnedAt = returnedAt,
                BorrowFee = record.Book.BorrowFee,
                FineAmount = fineAmount,
                LateDays = lateDays
            };
        }

        //public async Task<CreateBorrowVM> ReturnBook(int borrowingRecordId, string librarianId)
        //{
        //    var record = _unitOfWork.BorrowingRecords
        //        .GetActiveBorrowing(borrowingRecordId);
        //    if (record == null)
        //    {
        //        return new CreateBorrowVM
        //        {
        //            IsSuccess = false,
        //            Message = "Borrowing record not found."
        //        };
        //    }
        //    var returnedAt = DateTime.Now;
        //    int lateDays = 0;
        //    if (record.DueDate.HasValue && returnedAt > record.DueDate.Value)
        //    {
        //        lateDays = (returnedAt.Date - record.DueDate.Value.Date).Days;
        //    }
        //    decimal fineAmount = lateDays * record.Book.DailyFineRate;
        //    var borrowTransaction = new Transaction
        //    {
        //        UserId = record.UserId,
        //        LibrarianId = librarianId,
        //        Amount = record.Book.BorrowFee,
        //        Type = TransactionType.BorrowFee,
        //        RecordedAt = DateTime.Now,
        //        IsPaid = true,
        //        PaidAt = DateTime.Now,
        //        Notes = $"Borrow fee for book: {record.Book.Title}"
        //    };
        //    _unitOfWork.Transactions.Add(borrowTransaction);
        //    Transaction fineTransaction = null;
        //    if (fineAmount > 0)
        //    {
        //        fineTransaction = new Transaction
        //        {
        //            UserId = record.UserId,
        //            LibrarianId = librarianId,
        //            Amount = fineAmount,
        //            Type = TransactionType.Fine,
        //            RecordedAt = DateTime.Now,
        //            IsPaid = true,
        //            PaidAt = DateTime.Now,
        //            Notes = $"Late return fine for book: {record.Book.Title}"
        //        };
        //        _unitOfWork.Transactions.Add(fineTransaction);
        //    }
        //    _unitOfWork.Save();
        //    record.ReturnedAt = returnedAt;
        //    record.AccruedFine = fineAmount;
        //    record.BorrowingFeeTransactionId = borrowTransaction.Id;
        //    if (fineTransaction != null)
        //    {
        //        record.FineTransactionId = fineTransaction.Id;
        //    }
        //    record.ProcessedByLibrarianId = librarianId;
        //    record.Status = fineAmount > 0
        //        ? BorrowingStatus.ReturnedOverdue
        //        : BorrowingStatus.Returned;

        //    record.Book.AvailableCopies++;

        //    _unitOfWork.Save();
        //    return new CreateBorrowVM
        //    {
        //        IsSuccess = true,
        //        Message = fineAmount > 0
        //            ? $"Book returned successfully with fine: {fineAmount:C}"
        //            : "Book returned successfully.",
        //        BorrowingRecordId = record.Id
        //    };
        //}
        public async Task<CreateBorrowVM> ReturnBook(int borrowingRecordId, string librarianId,
                                             bool isDamaged = false, decimal damageFee = 0,
                                             string? notes = null)
        {
            var record = _unitOfWork.BorrowingRecords.GetActiveBorrowing(borrowingRecordId);
            if (record == null)
                return new CreateBorrowVM { IsSuccess = false, Message = "Borrowing record not found." };

            var returnedAt = DateTime.Now;
            int lateDays = 0;
            if (record.DueDate.HasValue && returnedAt > record.DueDate.Value)
                lateDays = (returnedAt.Date - record.DueDate.Value.Date).Days;

            decimal fineAmount = lateDays * record.Book.DailyFineRate;

            // Borrow Fee Transaction
            var borrowTransaction = new Transaction
            {
                UserId = record.UserId,
                LibrarianId = librarianId,
                Amount = record.Book.BorrowFee,
                Type = TransactionType.BorrowFee,
                RecordedAt = DateTime.Now,
                IsPaid = true,
                PaidAt = DateTime.Now,
                Notes = $"Borrow fee for book: {record.Book.Title}"
            };
            _unitOfWork.Transactions.Add(borrowTransaction);

            // Late Fine Transaction
            Transaction? fineTransaction = null;
            if (fineAmount > 0)
            {
                fineTransaction = new Transaction
                {
                    UserId = record.UserId,
                    LibrarianId = librarianId,
                    Amount = fineAmount,
                    Type = TransactionType.Fine,
                    RecordedAt = DateTime.Now,
                    IsPaid = true,
                    PaidAt = DateTime.Now,
                    Notes = $"Late return fine for book: {record.Book.Title}"
                };
                _unitOfWork.Transactions.Add(fineTransaction);
            }

            // Damage Transaction
            Transaction? damageTransaction = null;
            if (isDamaged && damageFee > 0)
            {
                damageTransaction = new Transaction
                {
                    UserId = record.UserId,
                    LibrarianId = librarianId,
                    Amount = damageFee,
                    Type = TransactionType.Damaged,
                    RecordedAt = DateTime.Now,
                    IsPaid = true,
                    PaidAt = DateTime.Now,
                    Notes = $"Damage fee for book: {record.Book.Title}"
                };
                _unitOfWork.Transactions.Add(damageTransaction);
            }

            _unitOfWork.Save();

            record.ReturnedAt = returnedAt;
            record.AccruedFine = fineAmount + (isDamaged ? damageFee : 0);
            record.BorrowingFeeTransactionId = borrowTransaction.Id;
            record.FineTransactionId = fineTransaction?.Id;
            record.ProcessedByLibrarianId = librarianId;
            record.Notes = notes;

            record.Status = isDamaged
                ? BorrowingStatus.ReturnedDamaged
                : fineAmount > 0
                    ? BorrowingStatus.ReturnedOverdue
                    : BorrowingStatus.Returned;

            record.Book.AvailableCopies++;

            _unitOfWork.Save();

            _reservationService.CheckAndAssignReservationOnReturn(record.BookId);

            var message = isDamaged
                ? $"Book returned with damage. Damage fee: {damageFee:C}" +
                  (fineAmount > 0 ? $" + Late fine: {fineAmount:C}" : "")
                : fineAmount > 0
                    ? $"Book returned successfully. Late fine: {fineAmount:C}"
                    : "Book returned successfully.";

            return new CreateBorrowVM
            {
                IsSuccess = true,
                Message = message,
                BorrowingRecordId = record.Id
            };
        }

        public async Task<CreateBorrowVM> CreateBorrowingOnPickup(int reservationId, string librarianId)
        {
            var reservation = _unitOfWork.Reservations.GetById(reservationId);
            if (reservation == null) return new CreateBorrowVM { IsSuccess = false, Message = "Reservation not found." };

            if (reservation.Status != ReservationStatus.Ready) return new CreateBorrowVM { IsSuccess = false, Message = "Reservation is not ready for pickup." };

            var book = _unitOfWork.Books.GetById(reservation.BookId);
            if (book == null) return new CreateBorrowVM { IsSuccess = false, Message = "Book not found." };

            var subscription = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionByUserAsync(reservation.UserId);
            var plan = subscription?.Plan ?? _unitOfWork.SubscriptionPlans.GetByName("Free");

            if (plan == null)
                return new CreateBorrowVM { IsSuccess = false, Message = "Subscription plan not found." };

            var borrowRecord = new BorrowingRecord
            {
                UserId = reservation.UserId,
                BookId = reservation.BookId,
                Status = BorrowingStatus.Active,
                RequestedAt = reservation.ReservedAt,
                CheckedOutAt = DateTime.Now,
                DueDate = DateTime.Now.AddDays(plan.LoanDurationDays),
                ProcessedByLibrarianId = librarianId
            };

            _unitOfWork.BorrowingRecords.Add(borrowRecord);
            _unitOfWork.Save();
            return new CreateBorrowVM
            {
                IsSuccess = true,
                BorrowingRecordId = borrowRecord.Id,
                Message = "Borrowing started successfully."
            };
        }

        public IEnumerable<BorrowingRecordVM> GetByUserId(string userId)
        {
            return _unitOfWork.BorrowingRecords.GetByUserId(userId)
                .Select(r => new BorrowingRecordVM
                {
                    Id = r.Id,
                    BorrowerName = r.User.FullName,
                    BookTitle = r.Book.Title,
                    Status = r.Status,
                    RequestedAt = r.RequestedAt,
                    CheckedOutAt = r.CheckedOutAt,
                    DueDate = r.DueDate
                }).ToList();
        }

        public async Task<CreateBorrowVM> CancelBorrowRequest(int borrowingRecordId, string userId)
        {
            var record = _unitOfWork.BorrowingRecords.GetById(borrowingRecordId);
            if (record == null)
                return new CreateBorrowVM { IsSuccess = false, Message = "Request not found." };

            if (record.UserId != userId)
                return new CreateBorrowVM { IsSuccess = false, Message = "You do not have permission to cancel this request." };

            if (record.Status != BorrowingStatus.Pending)
                return new CreateBorrowVM { IsSuccess = false, Message = "Only pending requests can be cancelled." };

            record.Status = BorrowingStatus.Cancelled;
           
            _unitOfWork.Save();
            return new CreateBorrowVM { IsSuccess = true, Message = "Your borrow request has been cancelled." };
        }
    }
}

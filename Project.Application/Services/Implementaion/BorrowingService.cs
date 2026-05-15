using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.BorrowingRecord;
using Project.Core;
using Project.Core.Enums;
using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Implementaion
{
    public class BorrowingService : IBorrowingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BorrowingService(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
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
            SubscriptionPlan plan;
            DateTime startDate;
            if (subscription == null)
            {
                plan = _unitOfWork.SubscriptionPlans.GetByName("Free");
                if (plan == null)
                {
                    return new CreateBorrowVM
                    {
                        IsSuccess = false,
                        Message = "Free subscription plan is not configured."
                    };
                }
                startDate = record.RequestedAt;
            }
            else
            {
                plan = subscription.Plan;
                startDate = subscription.StartDate;
            }
            var endDate = subscription?.EndDate ?? DateTime.UtcNow;
            var borrowCount = _unitOfWork.BorrowingRecords
                .CountUserBorrowsThisMonth(record.UserId, startDate, endDate);
            if (borrowCount >= plan.MonthlyBorrowLimit)
            {
                return new CreateBorrowVM
                {
                    IsSuccess = false,
                    Message = "User reached borrow limit for current subscription."
                };
            }

            var loanDays = plan.LoanDurationDays;

            record.Status = BorrowingStatus.Active;
            record.CheckedOutAt = DateTime.UtcNow;
            record.DueDate = DateTime.UtcNow.AddDays(loanDays);
            record.ProcessedByLibrarianId = librarianId;

            book.AvailableCopies--;
            _unitOfWork.Save();
            return new CreateBorrowVM
            {
                IsSuccess = true,
                Message = "Borrow request approved successfully.",
                BorrowingRecordId = record.Id
            };
        }

        public CreateBorrowVM BorrowBook(int bookId, string userId)
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
            var borrowRequest = new BorrowingRecord
            {
                BookId = bookId,
                UserId = userId,
                Status = BorrowingStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            _unitOfWork.BorrowingRecords.Add(borrowRequest);
            _unitOfWork.Save();
            return new CreateBorrowVM { BorrowingRecordId = borrowRequest.Id, IsSuccess = true, Message = "Request submitted successfully." };
        }

        public IEnumerable<BorrowingRecordVM> GetAllForLibrarian()
        {
            var records = _unitOfWork.BorrowingRecords.GetAll();
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
                ProcessedByLibrarianName = record.ProcessedByLibrarian?.FullName,
                Notes = record.Notes
            };
        }

        public IEnumerable<BorrowingRecordVM> GetPendingRequests()
        {
            var records = _unitOfWork.BorrowingRecords.GetAllPendingForLibrarian();

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
            var returnedAt = DateTime.UtcNow;
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

        public async Task<CreateBorrowVM> ReturnBook(int borrowingRecordId, string librarianId)
        {
            var record = _unitOfWork.BorrowingRecords
                .GetActiveBorrowing(borrowingRecordId);
            if (record == null)
            {
                return new CreateBorrowVM
                {
                    IsSuccess = false,
                    Message = "Borrowing record not found."
                };
            }
            var returnedAt = DateTime.UtcNow;
            int lateDays = 0;
            if (record.DueDate.HasValue && returnedAt > record.DueDate.Value)
            {
                lateDays = (returnedAt.Date - record.DueDate.Value.Date).Days;
            }
            decimal fineAmount = lateDays * record.Book.DailyFineRate;
            var borrowTransaction = new Transaction
            {
                UserId = record.UserId,
                LibrarianId = librarianId,
                Amount = record.Book.BorrowFee,
                Type = TransactionType.BorrowFee,
                RecordedAt = DateTime.UtcNow,
                IsPaid = true,
                PaidAt = DateTime.UtcNow,
                Notes = $"Borrow fee for book: {record.Book.Title}"
            };
            _unitOfWork.Transactions.Add(borrowTransaction);
            Transaction fineTransaction = null;
            if (fineAmount > 0)
            {
                fineTransaction = new Transaction
                {
                    UserId = record.UserId,
                    LibrarianId = librarianId,
                    Amount = fineAmount,
                    Type = TransactionType.Fine,
                    RecordedAt = DateTime.UtcNow,
                    IsPaid = true,
                    PaidAt = DateTime.UtcNow,
                    Notes = $"Late return fine for book: {record.Book.Title}"
                };
                _unitOfWork.Transactions.Add(fineTransaction);
            }
            _unitOfWork.Save();
            record.ReturnedAt = returnedAt;
            record.AccruedFine = fineAmount;
            record.BorrowingFeeTransactionId = borrowTransaction.Id;
            if (fineTransaction != null)
            {
                record.FineTransactionId = fineTransaction.Id;
            }
            record.ProcessedByLibrarianId = librarianId;
            record.Status = fineAmount > 0
                ? BorrowingStatus.ReturnedOverdue
                : BorrowingStatus.Returned;

            record.Book.AvailableCopies++;

            _unitOfWork.Save();
            return new CreateBorrowVM
            {
                IsSuccess = true,
                Message = fineAmount > 0
                    ? $"Book returned successfully with fine: {fineAmount:C}"
                    : "Book returned successfully.",
                BorrowingRecordId = record.Id
            };
        }
    }
}

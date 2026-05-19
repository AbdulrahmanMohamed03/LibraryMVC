using Project.Application.ViewModels.BorrowingRecord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface IBorrowingService 
    {
        IEnumerable<BorrowingRecordVM> GetAllForLibrarian(string? search = null);
        BorrowingDetailsVM GetDetails(int id);
        Task<CreateBorrowVM> BorrowBook(int bookId, string userId);
        Task<CreateBorrowVM> ApproveRequest(int borrowingRecordId, string librarianId);
        IEnumerable<BorrowingRecordVM> GetPendingRequests(string? search = null);
        ReturnBorrowVM GetReturnDetails(int borrowingRecordId);

        Task<CreateBorrowVM> ReturnBook(int borrowingRecordId, string librarianId, bool isDamaged = false, decimal damageFee = 0, string? notes = null);

        bool UserHasActiveBorrowForBook(string userId, int bookId);

        Task<CreateBorrowVM> CreateBorrowingOnPickup(int reservationId, string librarianId);
        IEnumerable<BorrowingRecordVM> GetByUserId(string userId);
        Task<CreateBorrowVM> CancelBorrowRequest(int borrowingRecordId, string userId);
    }
}

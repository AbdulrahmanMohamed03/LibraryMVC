using Project.Application.ViewModels.BorrowingRecord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface IBorrowingService 
    {
        IEnumerable<BorrowingRecordVM> GetAllForLibrarian();
        BorrowingDetailsVM GetDetails(int id);
        CreateBorrowVM BorrowBook(int bookId, string userId);
        Task<CreateBorrowVM> ApproveRequest(int borrowingRecordId, string librarianId);
        IEnumerable<BorrowingRecordVM> GetPendingRequests();
        ReturnBorrowVM GetReturnDetails(int borrowingRecordId);

        Task<CreateBorrowVM> ReturnBook(int borrowingRecordId, string librarianId);

        bool UserHasActiveBorrowForBook(string userId, int bookId);
    }
}

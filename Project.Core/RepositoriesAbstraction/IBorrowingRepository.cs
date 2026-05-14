using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.RepositoriesAbstraction
{
    public interface IBorrowingRepository : IBaseRepository<BorrowingRecord>
    {
        IEnumerable<BorrowingRecord> GetAll();
        BorrowingRecord GetWithDetails(int id);
        IEnumerable<BorrowingRecord> GetByUserId(string userId);
        IEnumerable<BorrowingRecord> GetAllPendingForLibrarian();
        IEnumerable<BorrowingRecord> GetAllOverDue();

        bool ExistingActiveBorrowingForBook(int bookId, string userId);
        bool ExistingPendingBorrowingForBook(int bookId, string userId);
        int CountUserBorrowsThisMonth(string userId, DateTime startDate, DateTime endDate);
        BorrowingRecord GetActiveBorrowing(int id);
    }
}

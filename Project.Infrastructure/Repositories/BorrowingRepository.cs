using Microsoft.EntityFrameworkCore;
using Project.Core.Enums;
using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project.Infrastructure.Repositories
{
    public class BorrowingRepository : BaseRepository<BorrowingRecord>, IBorrowingRepository
    {
        public BorrowingRepository(AppDbContext _context) : base(_context)
        {
        }

        public IEnumerable<BorrowingRecord> GetAllOverDue()
        {
            return _context.BorrowingRecords
                .Where(br => br.Status == BorrowingStatus.Active && br.DueDate < DateTime.Now)
                .OrderBy(br => br.DueDate)
                .ToList();
        }

        public IEnumerable<BorrowingRecord> GetAllPendingForLibrarian(string? search = null)
        {
            var query = _context.BorrowingRecords
                .Include(br => br.User)
                .Include(br => br.Book)
                .Where(br => br.Status == BorrowingStatus.Pending && br.ProcessedByLibrarianId == null)
                .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(br => br.User.NationalId.Contains(search));
            }
            return query.ToList();
        }

        public IEnumerable<BorrowingRecord> GetAll(string? search = null)
        {
            var query = _context.BorrowingRecords
                .Include(br => br.User)
                .Include(br => br.Book)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(br => br.User.NationalId.Contains(search));
            }

            return query.ToList();
        }

        public IEnumerable<BorrowingRecord> GetByUserId(string userId)
        {
            return _context.BorrowingRecords
                .Include (br => br.User)
                .Include(br => br.Book)
                .Where(br => br.UserId == userId)
                .OrderByDescending(br => br.RequestedAt)
                .ToList();
        }

        public BorrowingRecord GetWithDetails(int id)
        {
            return _context.BorrowingRecords
                .Include(br => br.User)
                .Include(br => br.Book).ThenInclude(b => b.Author)
                .Include(br => br.Book).ThenInclude(b => b.Category)
                .Include(br => br.BorrowingFeeTransaction)
                .Include(br => br.FineTransaction)
                .Include(br => br.ProcessedByLibrarian)
                .FirstOrDefault(br => br.Id == id);
        }

        public bool ExistingActiveBorrowingForBook(int bookId, string userId)
        {
            return _context.BorrowingRecords
                .Any(br => br.BookId == bookId && br.UserId == userId && br.Status == BorrowingStatus.Active);
        }

        public bool ExistingPendingBorrowingForBook(int bookId, string userId)
        {
            return _context.BorrowingRecords
                .Any(br => br.BookId == bookId && br.UserId == userId && br.Status == BorrowingStatus.Pending);
        }

        public int CountUserBorrowsThisMonth(string userId, DateTime startDate, DateTime endDate)
        {
            return _context.BorrowingRecords
                .Count(br => br.UserId == userId && br.RequestedAt >= startDate && br.RequestedAt < endDate);
        }

        public BorrowingRecord GetActiveBorrowing(int id)
        {
            return _context.BorrowingRecords
                .Include(br => br.User)
                .Include(br => br.Book)
                .FirstOrDefault(br =>
                    br.Id == id &&
                    br.Status == BorrowingStatus.Active);
        }
        public IEnumerable<BorrowingRecord> GetPendingByBookId(int bookId)
        {
            return _context.BorrowingRecords
                .Where(br => br.BookId == bookId && br.Status == BorrowingStatus.Pending)
                .OrderBy(br => br.RequestedAt)
                .ToList();
        }
    }
}

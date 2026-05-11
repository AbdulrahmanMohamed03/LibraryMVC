using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;


using Microsoft.EntityFrameworkCore;
using Project.Core.Enums;


namespace Project.Infrastructure.Repositories
{
    public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // ── البحث عن أقدم حجز (FIFO) ──────────────────────────────────────────
        public Reservation? GetOldestPendingForBook(int bookId)
            => _context.Reservations
                       .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Pending)
                       .OrderBy(r => r.ReservedAt)
                       .FirstOrDefault();

        public IEnumerable<Reservation> GetByBookAndStatus(int bookId, ReservationStatus status)
            => _context.Reservations
                       .Where(r => r.BookId == bookId && r.Status == status)
                       .OrderBy(r => r.ReservedAt)
                       .ToList();

        public int GetPendingQueueDepth(int bookId)
            => _context.Reservations
                       .Count(r => r.BookId == bookId && r.Status == ReservationStatus.Pending);

        // ── جلب حجوزات المستخدم مع تفاصيل الكتاب ──────────────────────────────
        public IEnumerable<Reservation> GetByUserWithDetails(string userId)
            => _context.Reservations
                       .Include(r => r.Book)
                           .ThenInclude(b => b.Author)
                       .Include(r => r.Book)
                           .ThenInclude(b => b.Category)
                       .Where(r => r.UserId == userId)
                       .OrderByDescending(r => r.ReservedAt)
                       .ToList();

        // ── التأكد من عدم تكرار الحجز لنفس الشخص ──────────────────────────────
        public Reservation? GetActiveForUserAndBook(string userId, int bookId)
            => _context.Reservations
                       .FirstOrDefault(r =>
                           r.UserId == userId &&
                           r.BookId == bookId &&
                           (r.Status == ReservationStatus.Pending ||
                            r.Status == ReservationStatus.Ready));

        // ── البحث عن الحجوزات المنتهية ────────────────────────────────────────
        public IEnumerable<Reservation> GetExpired()
        {
            var now = DateTime.UtcNow;
            return _context.Reservations
                           .Include(r => r.Book)
                           .Where(r =>
                               r.ExpiresAt <= now &&
                               (r.Status == ReservationStatus.Pending ||
                                r.Status == ReservationStatus.Ready))
                           .ToList();
        }

        // ── التحقق من الغرامات ────────────────────────────────────────────────
        public bool UserHasUnpaidFines(string userId)
            => _context.BorrowingRecords
                       .Any(br =>
                           br.UserId == userId &&
                           br.Status != BorrowingStatus.Returned &&
                           br.AccruedFine > 0);

        public bool UserHasActiveSubscription(string userId)
        {
            var now = DateTime.UtcNow;
            return _context.UserSubscriptions
                           .Any(s => s.UserId == userId
                                  && s.IsActive
                                  && s.EndDate >= now);
        }

        public async Task<IEnumerable<Reservation>> GetPendingByBookIdAsync(int bookId)
        {
            return await _context.Reservations
                .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Pending)
                .ToListAsync();
        }
    }
}
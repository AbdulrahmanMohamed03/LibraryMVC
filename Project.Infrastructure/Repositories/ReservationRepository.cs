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

        // ── looking for oldest reservation (FIFO) ──────────────────────────────────────────
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

        // ── get the user reservations with its details ──────────────────────────────
        public IEnumerable<Reservation> GetByUserWithDetails(string userId)
            => _context.Reservations
                       .Include(r => r.Book)
                           .ThenInclude(b => b.Author)
                       .Include(r => r.Book)
                           .ThenInclude(b => b.Category)
                       .Where(r => r.UserId == userId)
                       .OrderByDescending(r => r.ReservedAt)
                       .ToList();

        // ── confirm that the same user cannot reserve the same books twice ──────────────────────────────
        public Reservation? GetActiveForUserAndBook(string userId, int bookId)
            => _context.Reservations
                       .FirstOrDefault(r =>
                           r.UserId == userId &&
                           r.BookId == bookId &&
                           (r.Status == ReservationStatus.Pending ||
                            r.Status == ReservationStatus.Ready));

        // ── search for expired reservations ────────────────────────────────────────
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

     
        /// All reservations for all users, with full navigation properties.
        /// Ordered: active statuses first (Pending, Ready), then by date.
       
        public IEnumerable<Reservation> GetAllWithDetails()
            => _context.Reservations
                       .Include(r => r.User)
                       .Include(r => r.Book).ThenInclude(b => b.Author)
                       .Include(r => r.Book).ThenInclude(b => b.Category)
                       .OrderBy(r =>
                           r.Status == ReservationStatus.Ready ? 0 :
                           r.Status == ReservationStatus.Pending ? 1 :
                           r.Status == ReservationStatus.Fulfilled ? 2 : 3)
                       .ThenByDescending(r => r.ReservedAt)
                       .ToList();
    }
}
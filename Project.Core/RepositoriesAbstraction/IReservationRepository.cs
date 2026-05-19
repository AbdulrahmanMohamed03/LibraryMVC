using Project.Core.Enums;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.RepositoriesAbstraction
{
    public interface IReservationRepository : IBaseRepository<Reservation>
    {
        Reservation? GetOldestPendingForBook(int bookId);
        IEnumerable<Reservation> GetByBookAndStatus(int bookId, ReservationStatus status);

        
        int GetPendingQueueDepth(int bookId);

        IEnumerable<Reservation> GetByUserWithDetails(string userId);

      
        Reservation? GetActiveForUserAndBook(string userId, int bookId);
        IEnumerable<Reservation> GetExpired();

        bool UserHasUnpaidFines(string userId);

        bool UserHasActiveSubscription(string userId);

        Task<IEnumerable<Reservation>> GetPendingByBookIdAsync(int bookId);


        IEnumerable<Reservation> GetAllWithDetails();
        int GetReadyReservations();
    }
}

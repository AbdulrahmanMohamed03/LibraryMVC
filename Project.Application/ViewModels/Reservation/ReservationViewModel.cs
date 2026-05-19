using Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.Reservation
{
 
    public class ReservationViewModel
    {
        public int Id { get; set; }

        // Book info
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string BookAuthor { get; set; } = null!;
        public string? BookCoverUrl { get; set; }

        // User info (used in librarian view)
        public string UserId { get; set; } = null!;
        public string UserFullName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;

        // Reservation data
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public ReservationStatus Status { get; set; }

        //Computed display helpers

        public string ReservedAtDisplay => ReservedAt.ToString("MMM dd, yyyy");
        public string ExpiresAtDisplay => ExpiresAt.ToString("MMM dd, yyyy HH:mm");

        public string StatusLabel => Status switch
        {
            ReservationStatus.Pending => "In Queue",
            ReservationStatus.Ready => "Ready for Pickup",
            ReservationStatus.Fulfilled => "Fulfilled",
            ReservationStatus.Cancelled => "Cancelled",
            _ => Status.ToString()
        };

        public string StatusBadgeClass => Status switch
        {
            ReservationStatus.Pending => "badge-warning",
            ReservationStatus.Ready => "badge-success",
            ReservationStatus.Fulfilled => "badge-secondary",
            ReservationStatus.Cancelled => "badge-danger",
            _ => "badge-secondary"
        };

        //public bool IsCancellable =>
        //    Status == ReservationStatus.Pending ||
        //    Status == ReservationStatus.Ready;


        public bool IsCancellable => Status == ReservationStatus.Pending || Status == ReservationStatus.Ready;

        public bool IsExpiringSoon =>
            Status == ReservationStatus.Ready &&
            ExpiresAt <= DateTime.Now.AddHours(24);

        public bool IsExpired => ExpiresAt < DateTime.Now;
    }
}
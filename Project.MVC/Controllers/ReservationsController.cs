using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Services.Implementaion;
using Project.Application.Services.Interfaces;
using Project.Core.Models;
using System.Security.Claims;

namespace Project.Web.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IBorrowingService _borrowingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(
            IReservationService reservationService,
            UserManager<ApplicationUser> userManager,
            IBorrowingService borrowingService)
        {
            _reservationService = reservationService;
            _userManager = userManager;
            _borrowingService = borrowingService;
        }

        // ── Helper ────────────────────────────────────────────────────────────
        private string CurrentUserId =>
            _userManager.GetUserId(User)
            ?? throw new InvalidOperationException("User session is invalid.");

        // ─────────────────────────────────────────────────────────────────────
        // INDEX: Reservations dashboard filtered by user role
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Librarian"))
            {
                var adminVm = _reservationService.GetAllReservationsForAdmin();
                return View(adminVm);
            }

            var userVm = _reservationService.GetUserReservations(CurrentUserId);
            return View(userVm);
        }

        // ─────────────────────────────────────────────────────────────────────
        // CONFIRM: Show confirmation page before placing a reservation
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Confirm(int id)
        {
            var validationResult = _reservationService.CheckFormEligibility(id, CurrentUserId);

            if (!validationResult.IsSuccess)
            {
                TempData["ErrorMessage"] = validationResult.Message;
                return RedirectToAction("Details", "Books", new { id = id });
            }

            var vm = _reservationService.GetPlaceReservationForm(id, CurrentUserId);
            return View(vm);
        }

        // ─────────────────────────────────────────────────────────────────────
        // CREATE (POST): Place the reservation
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(int bookId)
        {
            var result = _reservationService.PlaceReservation(bookId, CurrentUserId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            TempData["SuccessMessage"] =
                $"Your reservation for '{result.Message}' has been placed. " +
                "You'll be notified when a copy is ready for pickup.";
            return RedirectToAction(nameof(Index));
        }

        // ─────────────────────────────────────────────────────────────────────
        // CANCEL (POST): Cancel a reservation (User view)
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Cancel(int id)
        {
            var result = _reservationService.CancelReservation(id, CurrentUserId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Your reservation has been cancelled successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ─────────────────────────────────────────────────────────────────────
        // ADMIN CANCEL (POST): Librarian cancel any user's reservation
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult AdminCancel(int id, string reservationUserId)
        {
            var result = _reservationService.CancelReservation(id, reservationUserId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Reservation cancelled successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ─────────────────────────────────────────────────────────────────────
        // MAINTENANCE: Expiry sweep (Admin/Librarian only)
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult ExpireSweep()
        {
            var count = _reservationService.ExpireOverdueReservations();
            TempData["SuccessMessage"] = count > 0
                ? $"{count} overdue reservation(s) expired and re-processed."
                : "No overdue reservations found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Pickup(int reservationId)
        {
            var librarianId = _userManager.GetUserId(User);
            var borrowResult = await _borrowingService.CreateBorrowingOnPickup(reservationId, librarianId);
            if (!borrowResult.IsSuccess)
            {
                TempData["ErrorMessage"] = borrowResult.Message;
                return RedirectToAction(nameof(Index));
            }
            var fulfillResult = _reservationService.MarkAsFulfilled(reservationId);
            if (!fulfillResult.IsSuccess)
            {
                TempData["ErrorMessage"] = fulfillResult.Message;
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Book picked up successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
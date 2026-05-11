using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Services.Interfaces;
using Project.Core.Models;

namespace Project.Web.Controllers
{
    //[Authorize]   
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(
            IReservationService reservationService,
            UserManager<ApplicationUser> userManager)
        {
            _reservationService = reservationService;
            _userManager = userManager;
        }

        // ── Helper ────────────────────────────────────────────────────────────
        //private string CurrentUserId =>
        //    _userManager.GetUserId(User)
        //    ?? throw new InvalidOperationException("User session is invalid.");

        private string CurrentUserId
        {
            get
            {
                
                // return _userManager.GetUserId(User) ?? throw new InvalidOperationException("...");

                
                return "fb82915b-4ae3-44d7-bdab-8aa6f7f60462"; 
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // INDEX: My Reservations dashboard
        // ─────────────────────────────────────────────────────────────────────
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            var vm = _reservationService.GetUserReservations(CurrentUserId);
            return View(vm);
        }

        // ─────────────────────────────────────────────────────────────────────
        // CONFIRM: Show confirmation page before placing a reservation
        // ─────────────────────────────────────────────────────────────────────

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Confirm(int id)
        {
            try
            {

                var vm = _reservationService.GetPlaceReservationForm(id, CurrentUserId);
                return View(vm);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Books", new { id = id });
            }
        }


        //public IActionResult Confirm(int id)
        //{

        //    // var vm = _reservationService.GetPlaceReservationForm(id, CurrentUserId);


        //    var fakeVm = new Project.Application.ViewModels.Reservation.PlaceReservationViewModel
        //    {
        //        BookId = id,
        //        BookTitle = "Test Book"
        //    };

        //    return View(fakeVm);
        //}
        // ─────────────────────────────────────────────────────────────────────
        // CREATE (POST): Place the reservation
        // ─────────────────────────────────────────────────────────────────────

        //[HttpPost]
        //[ValidateAntiForgeryToken]

        //public IActionResult Create(int bookId)
        //{
        //    try
        //    {
        //        var result = _reservationService.PlaceReservation(bookId, CurrentUserId);
        //        TempData["SuccessMessage"] =
        //            $"Your reservation for '{result.BookTitle}' has been placed. " +
        //            $"You are #{_reservationService.GetUserReservations(CurrentUserId)
        //                .Reservations.Count(r => r.BookId == bookId)} in the queue.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        TempData["ErrorMessage"] = ex.Message;
        //        return RedirectToAction("Details", "Books", new { id = bookId });
        //    }
        //}

        [HttpPost]
        [AllowAnonymous] 
        [ValidateAntiForgeryToken]
        public IActionResult Create(int bookId)
        {
            try
            {
             
                var userId = "37c85c86-de37-422e-adbf-35285e5b3c85";

                var result = _reservationService.PlaceReservation(bookId, userId);

                TempData["SuccessMessage"] = "Reservation placed successfully!";
                return RedirectToAction(nameof(Index)); 
            }
            catch (Exception ex)
            {
                
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index"); 
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // CANCEL (POST): Cancel a reservation
        // ─────────────────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            try
            {
                _reservationService.CancelReservation(id, CurrentUserId);
                TempData["SuccessMessage"] = "Your reservation has been cancelled.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // ─────────────────────────────────────────────────────────────────────
        // EXPIRE (POST): Admin-triggered expiry sweep
        // ─────────────────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult ExpireSweep()
        {
            var count = _reservationService.ExpireOverdueReservations();
            TempData["SuccessMessage"] = count > 0
                ? $"{count} overdue reservation(s) were expired and processed."
                : "No overdue reservations found.";
            return RedirectToAction(nameof(Index));
        }
    }
}
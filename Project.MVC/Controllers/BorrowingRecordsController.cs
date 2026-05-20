using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Services.Interfaces;
using System.Security.Claims;

namespace Project.MVC.Controllers
{
    public class BorrowingRecordsController : Controller
    {
        private readonly IBorrowingService _borrowingService;
        public BorrowingRecordsController(IBorrowingService _borrowingService)
        {
            this._borrowingService = _borrowingService;
        }
        public IActionResult Index(string? search)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Librarian"))
            {
                var data = _borrowingService.GetAllForLibrarian(search);
                ViewBag.Search = search;
                return View(data);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRecords = _borrowingService.GetByUserId(userId);
            return View(userRecords);
        }

        //public IActionResult Details(int id)
        //{
        //    var record = _borrowingService.GetDetails(id);
        //    if (record == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(record);
        //}

        public IActionResult Details(int id, string returnUrl = null)
        {

            var model =  _borrowingService.GetDetails(id);

            if (model == null) return NotFound();

            
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _borrowingService.BorrowBook(bookId, userId);
            if(!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Details", "Books", new { id = bookId });
            }
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index", "BorrowingRecords");
        }

        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult Pending(string? search)
        { 
            var data = _borrowingService.GetPendingRequests(search);
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var empId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _borrowingService.ApproveRequest(id, empId);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Pending");
            }
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Pending");
        }
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult Return(int id)
        {
            var vm = _borrowingService.GetReturnDetails(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);    
        }

        [HttpPost]
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> ConfirmReturn(int id, bool isDamaged = false, 
                                               decimal damageFee = 0, string? notes = null)
        {
            var librarianId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _borrowingService.ReturnBook(id, librarianId, isDamaged, damageFee, notes);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;

                return RedirectToAction("Index");
            }
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _borrowingService.CancelBorrowRequest(id, userId);

            if (!result.IsSuccess)
                TempData["ErrorMessage"] = result.Message;
            else
                TempData["SuccessMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}

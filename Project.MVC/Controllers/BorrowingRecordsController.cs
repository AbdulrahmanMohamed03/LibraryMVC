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
        public IActionResult Index()
        {
            var data = _borrowingService.GetAllForLibrarian();
            return View(data);
        }

        public IActionResult Details(int id)
        {
            var record = _borrowingService.GetDetails(id);
            if (record == null)
            {
                return NotFound();
            }
            return View(record);
        }

        [HttpPost]
        public IActionResult BorrowBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _borrowingService.BorrowBook(bookId, userId);
            if(!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Details", "Books", new { id = bookId });
            }
            return RedirectToAction("Index", "Books");
        }

        public IActionResult Pending()
        { 
            var data = _borrowingService.GetPendingRequests();
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
        public async Task<IActionResult> ConfirmReturn(int id)
        {
            var librarianId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _borrowingService.ReturnBook(id, librarianId);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;

                return RedirectToAction("Index");
            }
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}

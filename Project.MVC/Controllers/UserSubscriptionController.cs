using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.UserSubscription;
using System.Security.Claims;

namespace Project.MVC.Controllers
{
    //[Authorize]
    public class UserSubscriptionController : Controller
    {
        private readonly IUserSubscriptionService _subscriptionService;
        private readonly ISubscriptionPlanService _planService;
        public UserSubscriptionController(IUserSubscriptionService _subscriptionService, ISubscriptionPlanService _planService)
        {
            this._subscriptionService = _subscriptionService;
            this._planService = _planService;
        }

        // GET: /MySubscription/Index
        // Shows current active subscription or a message if none
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) return Challenge();

            //userId ??= "7f642de8-4e41-4127-954d-f4401ab4811e";
            await _subscriptionService.CheckAndDowngradeIfExpiredAsync(userId);
            var subscription = await _subscriptionService
                                     .GetActiveSubscriptionByUserAsync(userId);
            return View(subscription); 
        }

        // GET: /MySubscription/Plans
        // Shows all available plans to pick from
        [HttpGet]
        public IActionResult Plans()
        {
            var plans = _planService.GetAllSubscriptionPlans();
            return View(plans);
        }

        // POST: /MySubscription/Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(int planId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
                //userId = "7f642de8-4e41-4127-954d-f4401ab4811e"; 
            }

            var dto = new CreateUserSubscriptionVM
            {
                UserId = userId,
                PlanId = planId,
            };

            var result = await _subscriptionService.SubscribeAsync(dto);
            if (result == null)
            {
                TempData["Error"] = "Something went wrong. Please try again.";
                return RedirectToAction("Plans");
            }

            TempData["Success"] = "You have successfully subscribed!";
            return RedirectToAction("Index");
        }

        // POST: /MySubscription/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            //userId ??= "7f642de8-4e41-4127-954d-f4401ab4811e";
            var isCancelled = await _subscriptionService.CancelSubscriptionAsync(id, userId);
            if (!isCancelled) return NotFound();

            TempData["Success"] = "Your subscription has been cancelled.";
            return RedirectToAction("Index");
        }

        // POST: /MySubscription/Renew
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();
            //if (string.IsNullOrEmpty(userId)) userId = "7f642de8-4e41-4127-954d-f4401ab4811e";
            var isRenewed = await _subscriptionService.RenewSubscriptionAsync(userId);
            if (!isRenewed)
            {
                TempData["Error"] = "No active subscription found to renew.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Your subscription has been renewed!";
            return RedirectToAction("Index");
        }

        // GET: /MySubscription/History
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId ??= "7f642de8-4e41-4127-954d-f4401ab4811e";
            var history = await _subscriptionService
                                .GetSubscriptionHistoryAsync(userId);
            return View(history);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Project.Application.DTOs.SupscriptionPlan;
using Project.Application.Interfaces;
using Project.Application.Services;
using Project.Core.Models;

namespace Project.MVC.Controllers
{
    public class SubscriptionPlanController : Controller
    {
        private readonly ISubscriptionPlanService _planService;
        public SubscriptionPlanController(ISubscriptionPlanService _planService)
        {
            this._planService = _planService;
        }
        public IActionResult Index()
        {
            var plans = _planService.GetAllSubscriptionPlans();
            return View(plans);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateSubscriptionPlanDTO planDTO)
        {
            if(!ModelState.IsValid)
            {
                return View("CreateSubscriptionPlanView", planDTO);
            }

            var isCreated = _planService.CreateSubscriptionPlan(planDTO);
            if(!isCreated)
            {
                ModelState.AddModelError("Name", "Subscription Plan already exists");

                return View("CreateSubscriptionPlanView", planDTO);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var plan = _planService.GetSubscriptionPlanById(id);
            if (plan == null) return NotFound();
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UpdateSupscriptionPlanDTO planDTO)
        {
            if (!ModelState.IsValid)
                return View(planDTO);

            var isUpdated = _planService.UpdateSubscriptionPlan(planDTO);
            if (!isUpdated)
            {
                ModelState.AddModelError("Name", "Subscription Plan already exists");
                return View(planDTO);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var plan = _planService.GetSubscriptionPlanById(id);

            if (plan == null) return NotFound();

            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var isDeleted = _planService.DeleteSubscriptionPlan(id);

            if (!isDeleted) return NotFound();

            return RedirectToAction("Index");
        }
    }
}

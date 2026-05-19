using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Account;
using Project.Core.Models;

namespace Project.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        public AccountController(IAccountService service )
        {
            accountService = service;

        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await accountService.register(model);
            if (result.Success)
            {
                return RedirectToAction("Index", "Home");
            }
            foreach(var item in result.Errors)
            {
                ModelState.AddModelError("", item);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await accountService.Login(model);

                if (result.Succeeded)
                {
                    var user = await accountService.GetUserByEmail(model.Email);  
                    if(await accountService.IsUserInRoleAsync(user, "Admin") || await accountService.IsUserInRoleAsync(user, "Librarian"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "invalid email or password");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await accountService.Logout();
            return RedirectToAction("Login");
        }
    }
}

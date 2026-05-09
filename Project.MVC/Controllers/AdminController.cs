using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Role;
using Project.Application.ViewModels.User;

namespace Project.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateLibrarian()
        {
            ViewBag.Roles = new SelectList( await _adminService.GetAllRolesAsync(),"Name","Name");
            return View(new CreateLibrarianVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLibrarian(CreateLibrarianVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList("Name", "Name");
                return View(vm);
            }

            var result = await _adminService.CreateLibrarianAsync(vm);
            if (!result.IsSuccess)
            {
                ViewBag.Roles = new SelectList(await _adminService.GetAllRolesAsync(),"Name","Name");
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(CreateLibrarian));
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole()
        {
            ViewBag.Roles = new SelectList(
                await _adminService.GetAllRolesAsync(),"Name", "Name");
            return View(new AssignRoleVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(AssignRoleVm vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(await _adminService.GetAllRolesAsync(), "Name","Name");
                return View(vm);
            }

            var result = await _adminService.AssignRoleToUserAsync(vm);
            if (!result.IsSuccess)
            {
                ViewBag.Roles = new SelectList(await _adminService.GetAllRolesAsync(),"Name","Name");
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(AssignRole));
        }

        //[HttpGet]
        //public async Task<IActionResult> RemoveRole(string email)
        //{
        //    var roles = await _adminService.GetUserRolesAsync(email);
        //    ViewBag.Roles = new SelectList(roles,"RoleName","RoleName");
        //    var vm = new AssignRoleVm
        //    {
        //        Email = email
        //    };
        //    return View(vm);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RemoveRole(AssignRoleVm vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Roles = new SelectList(
        //            await _adminService.GetAllRolesAsync(),
        //            "Name",
        //            "Name");

        //        return View(vm);
        //    }

        //    var result = await _adminService.RemoveUserFromRoleAsync(vm);
        //    if (!result.IsSuccess)
        //    {
        //        ViewBag.Roles = new SelectList(await _adminService.GetAllRolesAsync(), "Name", "Name");
        //        ModelState.AddModelError("", result.Message);
        //        return View(vm);
        //    }

        //    TempData["Success"] = result.Message;
        //    return RedirectToAction(nameof(RemoveRole));
        //}
    }

}

using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin, Librarian")]
        public async Task<IActionResult> Index()
        {
            var data = await _adminService.GetDashboardDataAsync();
            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateLibrarian()
        {
            ViewBag.Roles = new SelectList(await _adminService.GetAllRolesAsync(), "Name", "Name");
            return View(new CreateLibrarianVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
                ViewBag.Roles = new SelectList(await _adminService.GetAllRolesAsync(), "Name", "Name");
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(CreateLibrarian));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRoles()
        {
            var vm = new ManageUserRolesVm
            {
                AllRoles = (await _adminService.GetAllRolesAsync())
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRoles(ManageUserRolesVm vm)
        {
            vm.AllRoles = (await _adminService.GetAllRolesAsync())
            .Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();
            ModelState.Remove("SelectedRole");
            if (!ModelState.IsValid)
                return View(vm);
            vm.Roles = await _adminService.GetUserRolesAsync(vm.Email);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(ManageUserRolesVm vm)
        {
            vm.AllRoles = (await _adminService.GetAllRolesAsync())
            .Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();
            ModelState.Remove("Roles");
            if (!ModelState.IsValid)
            {
                vm.Roles = await _adminService.GetUserRolesAsync(vm.Email);
                return View("ManageRoles", vm);
            }
            var result = await _adminService.AssignRoleToUserAsync(new AssignRoleVm
            {
                Email = vm.Email,
                RoleName = vm.SelectedRole
            });
            vm.Roles = await _adminService.GetUserRolesAsync(vm.Email);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return View("ManageRoles", vm);
            }
            TempData["Success"] = result.Message;
            return View("ManageRoles", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(string email, string roleName)
        {
            await _adminService.RemoveUserFromRoleAsync(new AssignRoleVm
            {
                Email = email,
                RoleName = roleName
            });
            return RedirectToAction(nameof(ManageRoles), new { email });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageLibrarians()
        {
            var librarians = await _adminService.GetAllLibrariansAsync();
            return View(librarians);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FireLibrarian(string id)
        {
            var result = await _adminService.FireLibrarianAsync(id);
            if (!result)
                return NotFound();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LibrarianDetails(string id)
        {
            var librarian = await _adminService.GetLibrarianDetailsAsync(id);
            if (librarian == null || !librarian.IsSuccess)
                return NotFound();
            return View(librarian);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditLibrarian(string id)
        {
            var vm = await _adminService.GetLibrarianForEditAsync(id);
            if (vm == null)
                return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditLibrarian(EditLibrarianVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            var result = await _adminService.EditLibrarianAsync(vm);
            if (!result)
                return View(vm);
            return RedirectToAction(nameof(ManageLibrarians));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReHireLibrarian(string id)
        {
            var result = await _adminService.ReHireLibrarianAsync(id);
            if (!result)
                return NotFound();

            return RedirectToAction(nameof(ManageLibrarians));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FiredLibrarians()
        {
            var data = await _adminService.GetFiredLibrariansAsync();
            return View("ManageLibrarians", data);
        }
    }
}

// Project.Web/Controllers/AuthorsController.cs
using Microsoft.AspNetCore.Mvc;
using Project.Application.DTOs.Author;
using Project.Application.Services;

namespace Project.Web.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly AuthorService _service;

        public AuthorsController(AuthorService service) => _service = service;

        // GET /Authors
        public IActionResult Index()
            => View(_service.GetAll());

        // GET /Authors/Details/5
        //public IActionResult Details(int id)
        //{
        //    var dto = _service.GetWithBooks(id);
        //    return dto is null ? NotFound() : View(dto);
        //}

        // GET /Authors/Create
        public IActionResult Create() => View();

        // POST /Authors/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(CreateAuthorDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            _service.Create(dto);
            return RedirectToAction(nameof(Index));
        }

        // GET /Authors/Edit/5
        public IActionResult Edit(int id)
        {
            var dto = _service.GetById(id);
            if (dto is null) return NotFound();

            return View(new UpdateAuthorDto
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Bio = dto.Bio,
                Nationality = dto.Nationality
            });
        }

        // POST /Authors/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(UpdateAuthorDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var result = _service.Update(dto);
            return result is null ? NotFound() : RedirectToAction(nameof(Index));
        }

        // GET /Authors/Delete/5
        public IActionResult Delete(int id)
        {
            var dto = _service.GetById(id);
            return dto is null ? NotFound() : View(dto);
        }

        // POST /Authors/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _service.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

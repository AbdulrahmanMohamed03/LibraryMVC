


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // عشان نستخدم Include لو حابة تجيب البيانات المرتبطة
using Project.Infrastructure.Data; // استبدلي هذا باسم الـ Namespace الصحيح لـ ITIContext عندك
using Project.MVC.Models;
using System.Diagnostics;

namespace Project.MVC.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories
                .Select(c => new Project.Application.ViewModels.Category.CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

          
            var catalogBooks = _context.Books
                .Select(b => new Project.Application.ViewModels.Book.BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author != null ? b.Author.FullName : "Unknown Author",
                    CoverImageUrl = b.CoverImageUrl,

        
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category != null ? b.Category.Name : "General",
                    Description = b.Description,
                    BorrowFee = b.BorrowFee, 
                    AvailableCopies = b.AvailableCopies, 
           
                })
                .ToList();

            var featuredBooks = catalogBooks.Where(b => b.IsAvailable).Take(1).ToList();

            ViewBag.Categories = categories;
            ViewBag.CatalogBooks = catalogBooks;
            ViewBag.FeaturedBooks = featuredBooks;
            ViewBag.ActiveFilter = "all"; 

           
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
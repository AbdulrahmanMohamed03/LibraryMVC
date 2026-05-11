using Microsoft.EntityFrameworkCore;
using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Repositories
{
    public class BookRepository :BaseRepository<Book>, IBookRepository
    {
        public BookRepository(AppDbContext context) : base(context) { }

        public IEnumerable<Book> GetAllWithDetails()
            => _context.Books
                       .Include(b => b.Author)
                       .Include(b => b.Category)
                      // .Where(b => !b.IsDeleted)  // if we add soft delete later
                       .ToList();

        public Book? GetWithDetails(int id)
            => _context.Books
                       .Include(b => b.Author)
                       .Include(b => b.Category)
                       .FirstOrDefault(b => b.Id == id);

        public IEnumerable<Book> GetByAuthor(int authorId)
            => _context.Books
                       .Include(b => b.Category)
                       .Where(b => b.AuthorId == authorId)
                       .ToList();

        public IEnumerable<Book> GetByCategory(int categoryId)
            => _context.Books
                       .Include(b => b.Author)
                       .Where(b => b.CategoryId == categoryId)
                       .ToList();
    }
}


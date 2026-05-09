using Microsoft.EntityFrameworkCore;
using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Repositories
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(AppDbContext context) : base(context)
        {
        }

        public Author GetByName(string fullName)
        {
            return _context.Authors
                .FirstOrDefault(a =>
                    a.FullName.ToLower() == fullName.ToLower());
        }

        public Author GetWithBooks(int id)
        {
            return _context.Authors
                           .Include(a => a.Books)
                           .FirstOrDefault(a => a.Id == id);
        }
    }
}


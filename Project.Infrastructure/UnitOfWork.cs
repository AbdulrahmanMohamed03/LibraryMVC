using Project.Core;
using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork( AppDbContext _context)
        {
            this._context = _context;
            Categories = new CategoryRepository(_context);
            Authors = new AuthorRepository(_context);

        }
        public ICategoryRepository Categories { get; private set; }
        public IAuthorRepository Authors { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}

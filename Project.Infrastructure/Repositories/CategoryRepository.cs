using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext _context) : base(_context)
        {
        }

        public int GetBookCount(int categoryId)
        {
            return _context.Set<Book>().Count(b => b.CategoryId == categoryId);
        }

        public Category GetByName(string name)
        {
            return _context.Categories.FirstOrDefault(c => c.Name == name);
        }

        public bool NameExists(string name, int? excludeId = null)
        {
            return _context.Categories
                .Any(c => c.Name.ToLower() == name.ToLower()
                     && (!excludeId.HasValue || c.Id != excludeId.Value)
                     && !c.IsDeleted);
        }


        public override void Delete(int id)
        {
            var category = GetById(id);
            if (category != null)
            {
                category.IsDeleted = true;
                Update(category);
            }
        }
    }
}

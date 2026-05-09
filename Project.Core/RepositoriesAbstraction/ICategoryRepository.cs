using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.RepositoriesAbstraction
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        public Category GetByName(string name);
        bool NameExists(string name, int? excludeId = null);
        int GetBookCount(int categoryId);
    }
}

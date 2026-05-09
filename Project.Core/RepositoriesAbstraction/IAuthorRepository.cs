using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.RepositoriesAbstraction
{
    public interface IAuthorRepository :IBaseRepository<Author>
    {
        Author GetByName(string fullName);
        Author GetWithBooks(int id);
    }
}

using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.RepositoriesAbstraction
{
    public interface IBookRepository :IBaseRepository<Book>
    {
        IEnumerable<Book> GetAllWithDetails();        
        Book? GetWithDetails(int id);                 
        IEnumerable<Book> GetByAuthor(int authorId);
        IEnumerable<Book> GetByCategory(int categoryId);
    }
}

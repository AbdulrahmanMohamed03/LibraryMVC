using Project.Application.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface IBookService
    {
        IEnumerable<BookViewModel> GetAll();
        BookViewModel? GetById(int id);
        BookFormViewModel GetCreateForm();           // returns empty form with dropdowns
        BookFormViewModel? GetEditForm(int id);      // returns pre-filled form with dropdowns
        BookViewModel Create(BookFormViewModel vm, string wwwRootPath);
        BookViewModel? Update(BookFormViewModel vm, string wwwRootPath);
        bool Delete(int id);
    }
}

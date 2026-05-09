using Project.Application.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        //bool CreateCategory(CategoryViewModel categoryVM);
        //IEnumerable<CategoryViewModel> GetAllCategories();

        CategoryIndexViewModel GetIndexViewModel(string? searchTerm = null, bool showDeleted = false);
        CategoryViewModel GetById(int id);
        CategoryFormViewModel GetFormViewModel(int id);
        CategoryViewModel Create(CategoryFormViewModel model, string actorId);
        CategoryViewModel Update(CategoryFormViewModel model, string actorId);
        void Delete(int id, string actorId);
        void Restore(int id, string actorId);
    }
}

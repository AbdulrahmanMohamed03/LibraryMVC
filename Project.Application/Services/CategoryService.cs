using Microsoft.Extensions.Logging;
using Project.Application.Interfaces;
using Project.Application.ViewModels;
using Project.Core;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services
{
    public class CategoryService : ICategoryService
    {
       
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(IUnitOfWork uow, ILogger<CategoryService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public CategoryIndexViewModel GetIndexViewModel(string? searchTerm = null, bool showDeleted = false)
        {
            var categories = _uow.Categories.GetAll()
                .Where(c => c.IsDeleted == showDeleted &&
                           (string.IsNullOrEmpty(searchTerm) ||
                            c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsDeleted = c.IsDeleted,
                    BookCount = _uow.Categories.GetBookCount(c.Id),
                    CreatedAt = c.CreatedAt
                }).ToList();

            return new CategoryIndexViewModel
            {
                Categories = categories,
                SearchTerm = searchTerm
            };
        }


        public CategoryViewModel GetById(int id)
        {
            var category = _uow.Categories.GetById(id);

            if (category == null || category.IsDeleted)
            {
                _logger.LogWarning($"Category with ID {id} was not found.");
                return null;
            }

            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                BookCount = _uow.Categories.GetBookCount(id)
            };
        }

        
        public CategoryFormViewModel GetFormViewModel(int id)
        {
            var category = _uow.Categories.GetById(id);

            if (category == null || category.IsDeleted)
                throw new Exception("Category doesn't exist or it is deleted");

            return new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        
        public CategoryViewModel Create(CategoryFormViewModel model, string actorId)
        {
            
            if (_uow.Categories.NameExists(model.Name))
            {
                _logger.LogWarning($"Actor {actorId} tried to create a duplicate category: {model.Name}");
                throw new Exception("Category already exists");
            }

            var category = new Category
            {
                Name = model.Name.Trim(),
                IsDeleted = false
            };

            _uow.Categories.Add(category);
            _uow.Save();

            _logger.LogInformation($"Actor {actorId} created new category: {category.Name}");

            return GetById(category.Id);
        }

       
        public CategoryViewModel Update(CategoryFormViewModel model, string actorId)
        {
            var category = _uow.Categories.GetById(model.Id);

            if (category == null) throw new Exception("Category doesn't exist for editing ");

       
            if (_uow.Categories.NameExists(model.Name, model.Id))
                throw new Exception("Category Name already exists.");

            var oldName = category.Name;
            category.Name = model.Name.Trim();

            _uow.Categories.Update(category);
            _uow.Save();

            _logger.LogInformation($"Actor {actorId} renamed Category {model.Id} from '{oldName}' to '{category.Name}'");

            return GetById(category.Id);
        }


        public void Delete(int id, string actorId)
        {
            
            var category = _uow.Categories.GetById(id);
            if (category == null)
            {
                _logger.LogWarning($"Delete failed: Category with ID {id} not found.");
                return;
            }

           
            int bookCount = _uow.Categories.GetBookCount(id);
            if (bookCount > 0)
            {
                _logger.LogWarning($"Actor {actorId} blocked from deleting category '{category.Name}' (ID: {id}) because it contains {bookCount} books.");

                throw new Exception($"Operation Failed: This category contains {bookCount} books. Please reassign or delete the books first.");
            }

           
            _uow.Categories.Delete(id);

            _uow.Save();

          
            _logger.LogInformation($"Actor {actorId} successfully soft-deleted Category {id} ({category.Name}).");
        }

        public void Restore(int id, string actorId)
        {
            var category = _uow.Categories.GetById(id);
            if (category != null)
            {
                category.IsDeleted = false;
                _uow.Categories.Update(category);
                _uow.Save();
                _logger.LogInformation($"Actor {actorId} restored Category {id}");
            }
        }
    }
}

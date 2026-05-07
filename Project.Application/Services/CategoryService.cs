using Project.Application.DTOs;
using Project.Application.Interfaces;
using Project.Core;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService( IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }
        public bool CreateCategory(CreateCategoryDTO categoryDto)
        {
            var foundCategory = _unitOfWork.Categories.GetByName(categoryDto.Name);

            if (foundCategory != null)
                return false;

            var category = new Category
            {
                Name = categoryDto.Name
            };

            _unitOfWork.Categories.Add(category);
            _unitOfWork.Save();

            return true;
        }

        public IEnumerable<CreateCategoryDTO> GetAllCategories()
        {
            var categories = _unitOfWork.Categories.GetAll();

            return categories.Select(c => new CreateCategoryDTO
            {
                Name = c.Name
            });
        }
    }
}

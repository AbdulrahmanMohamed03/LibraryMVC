using Project.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Interfaces
{
    public interface ICategoryService
    {
        bool CreateCategory(CreateCategoryDTO categoryDto);
        IEnumerable<CreateCategoryDTO> GetAllCategories();
    }
}

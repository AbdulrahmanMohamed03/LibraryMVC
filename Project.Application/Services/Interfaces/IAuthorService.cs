using Project.Application.ViewModels.Author;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Interfaces
{
    public interface IAuthorService
    {
        IEnumerable<AuthorDto> GetAll();
        AuthorDto? GetById(int id);
        AuthorDto? GetByName(string fullName);
        AuthorDto Create(CreateAuthorDto dto);
        AuthorDto? Update(UpdateAuthorDto dto);
        bool Delete(int id);
        AuthorWithBooksDto? GetWithBooks(int id);
    }
}

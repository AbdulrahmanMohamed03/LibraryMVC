using Project.Application.Services.Interfaces;
using Project.Application.ViewModels.Author;
using Project.Application.ViewModels.Book;
using Project.Core;
using Project.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.Services.Implementaion
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _uow;

        public AuthorService(IUnitOfWork uow) => _uow = uow;

        // ── Helpers ──────────────────────────────────────
        private static AuthorDto ToDto(Author a) => new AuthorDto
        {
            Id = a.Id,
            FullName = a.FullName,
            Bio = a.Bio,
            Nationality = a.Nationality,
            CreatedAt = a.CreatedAt
        };

        // ── Queries ──────────────────────────────────────
        public IEnumerable<AuthorDto> GetAll()
            => _uow.Authors.GetAll().Select(ToDto);

        public AuthorDto? GetById(int id)
        {
            var author = _uow.Authors.GetById(id);
            return author is null ? null : ToDto(author);
        }

        public AuthorDto? GetByName(string fullName)
        {
            var author = _uow.Authors.GetByName(fullName);
            return author is null ? null : ToDto(author);
        }
        // will exist in the future when we implement the GetWithBooks method in the repository
        public AuthorWithBooksDto? GetWithBooks(int id)
        {
            var author = _uow.Authors.GetWithBooks(id);
            if (author is null) return null;

            return new AuthorWithBooksDto
            {
                Id = author.Id,
                FullName = author.FullName,
                Bio = author.Bio,
                Nationality = author.Nationality,
                CreatedAt = author.CreatedAt,
                Books = author.Books?.Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title
                    // map other Book fields
                }) ?? Enumerable.Empty<BookViewModel>()
            };
        }

        // ── Commands ─────────────────────────────────────
        public AuthorDto Create(CreateAuthorDto dto)
        {
            var author = new Author
            {
                FullName = dto.FullName,
                Bio = dto.Bio,
                Nationality = dto.Nationality
            };
            _uow.Authors.Add(author);
            _uow.Save();
            return ToDto(author);
        }

        public AuthorDto? Update(UpdateAuthorDto dto)
        {
            var author = _uow.Authors.GetById(dto.Id);
            if (author is null) return null;

            author.FullName = dto.FullName;
            author.Bio = dto.Bio;
            author.Nationality = dto.Nationality;

            _uow.Authors.Update(author);
            _uow.Save();
            return ToDto(author);
        }

        public bool Delete(int id)
        {
            var author = _uow.Authors.GetById(id);
            if (author is null) return false;

            _uow.Authors.Delete(id);
            _uow.Save();
            return true;
        }

       
    }
}

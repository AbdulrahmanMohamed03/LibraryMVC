using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.Author
{
    public class AuthorWithBooksDto : AuthorDto
    {
        //untill making crud for books, we will not use this property, but it is here for future use
        //public IEnumerable<BookDto> Books { get; set; } = new List<BookDto>(); 
    }
}

using NationalBooks.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NationalBooks.Data.Interfaces
{
    public interface IBookService
    {
        List<Book> GetAllBooks();
    }
}

using NationalBooks.Data.Interfaces;
using NationalBooks.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NationalBooks.Data.Services
{
    public class BookService: IBookService
    {
        private readonly CosmosDBConnector _cosmos;

        public BookService(CosmosDBConnector cosmos)
        {
            this._cosmos = cosmos;
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books;

            //get the cookies from the database
            books = this._cosmos.RetrieveAllBooks();

            return books;
        }
    }
}

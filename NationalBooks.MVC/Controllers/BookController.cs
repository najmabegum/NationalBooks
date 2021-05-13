using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NationalBooks.Data.Interfaces;

namespace NationalBooks.MVC.Controllers
{
    public class BookController : Controller
    {
        private IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public IActionResult Index()
        {
            return View(_bookService.GetAllBooks());
        }
    }
}

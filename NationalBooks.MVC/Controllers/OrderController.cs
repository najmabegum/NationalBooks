using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NationalBooks.Data.Interfaces;
using NationalBooks.Data.Models;

namespace NationalBooks.MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;


        public OrderController(IOrderService orderService)
        {
            this._orderService = orderService;
        }

        public IActionResult Index()
        {
            System.Collections.Generic.List<Order> orders
                = this._orderService.GetAllOrders();

            return View(orders);
        }


        public IActionResult Detail(Guid id)
        {
            Order order = this._orderService.GetOrderById(id);

            return View(order);
        }

        public IActionResult CancelOrder(Guid id)
        {
            this._orderService.CancelOrder(id);

            return RedirectToAction("Index");
        }

        public IActionResult PlaceOrder(Guid id)
        {
            this._orderService.PlaceOrder(id);

            return RedirectToAction("Index");
        }

        public IActionResult AddBookToOrderLine(Guid bookId)
        {
            this._orderService.AddBookToOrder(bookId);

            return RedirectToAction("Index", "Order");
        }
    }
}

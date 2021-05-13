using NationalBooks.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NationalBooks.Data.Interfaces
{
    public interface IOrderService
    {
        void AddBookToOrder(Guid cookieId);

        List<Order> GetAllOrders();

        Order GetOrderById(Guid orderId);

        void CancelOrder(Guid orderId);

        void PlaceOrder(Guid orderId);
    }
}

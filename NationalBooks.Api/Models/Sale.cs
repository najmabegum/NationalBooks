using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationalBooks.Api.Models
{
    public class Sale
    {
        public string BookName { get; set; }
        public string Author { get; set; }

        public DateTime SaleFrom { get; set; }

        public int Percentage { get; set; }
    }
}

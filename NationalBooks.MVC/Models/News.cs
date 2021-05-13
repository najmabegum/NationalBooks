using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationalBooks.MVC.Models
{
    public class News
    {
        public List<Sale> SaleBooks { get; set; }
        public List<NewArrival> NewArrivalBooks { get; set; }
    }
}

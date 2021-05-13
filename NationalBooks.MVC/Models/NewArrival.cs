using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationalBooks.MVC.Models
{
    public class NewArrival
    {
        public string BookName { get; set; }
        public string Author { get; set; }

        public DateTime AvailableFrom { get; set; }
    }
}

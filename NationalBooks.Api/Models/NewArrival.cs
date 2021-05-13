using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationalBooks.Api.Models
{
    public class NewArrival
    {
        public string BookName { get; set; }
        public string Author { get; set; }

        public DateTime AvailableFrom{ get; set; }
    }
}

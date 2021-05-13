using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalBooks.Api.Models;

namespace NationalBooks.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalBooksNewsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<News> Get()
        {
            List<Sale> sales = new List<Sale>
            {
                new Sale{ BookName = "Educated", Author="Tara Westover", SaleFrom=new DateTime(2021,06,21),Percentage=20 },
                new Sale{ BookName = "Connections in Death", Author="J. D. Robb", SaleFrom=new DateTime(2021,06,22),Percentage=30 },                
                new Sale{ BookName = "Redemption", Author="David Baldacci", SaleFrom=new DateTime(2021,06,24),Percentage=40 },
                new Sale{ BookName = "A Better Man", Author="Louise Penny", SaleFrom=new DateTime(2021,06,25),Percentage=15 },
                new Sale{ BookName = "Blue Moon", Author="Lee Child", SaleFrom=new DateTime(2021,06,21),Percentage=50 }
            };

            List<NewArrival> newArrivals = new List<NewArrival>
            {
                new NewArrival{ BookName = "Finding Ashley", Author="Danielle Steel", AvailableFrom= new DateTime(2021,05,21)},
                new NewArrival{ BookName = "A Gambling Man", Author="David Baldacci", AvailableFrom=new DateTime(2021,05,21) },
                new NewArrival{ BookName = "The Body Keeps The Score", Author="Bessel van der Kolk", AvailableFrom=new DateTime(2021,05,22) },
                new NewArrival{ BookName = "The Rose Code", Author="Kate Quinn", AvailableFrom=new DateTime(2021,05,24) },
                new NewArrival{ BookName = "Nomadland", Author="Jessica Bruder", AvailableFrom=new DateTime(2021,05,26) }
            };

            News newsObject = new News();
            newsObject.SaleBooks = sales;
            newsObject.NewArrivalBooks = newArrivals;

            return newsObject;

        }
    }
}

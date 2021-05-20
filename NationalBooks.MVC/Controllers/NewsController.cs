using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NationalBooks.MVC.Models;
using Newtonsoft.Json;

namespace NationalBooks.MVC.Controllers
{
    public class NewsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = new System.Net.Http.HttpClient();
            string jsonResult = string.Empty;
            var request = new System.Net.Http.HttpRequestMessage();            
            request.RequestUri = new Uri("http://nationalbooks.api/api/NationalBooksNews/");
            var response = await client.SendAsync(request);
            News news = new News();
            jsonResult =
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            news = JsonConvert.DeserializeObject<News>(jsonResult);
            return View(news);
        }
    }
}

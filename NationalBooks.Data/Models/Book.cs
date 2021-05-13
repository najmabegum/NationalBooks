using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NationalBooks.Data.Models
{
    public class Book
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
    }
}

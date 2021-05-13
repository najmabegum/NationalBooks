using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NationalBooks.Data.Models
{
    public class OrderLine
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public Book Book { get; set; }
        public Order Order { get; set; }
    }
}

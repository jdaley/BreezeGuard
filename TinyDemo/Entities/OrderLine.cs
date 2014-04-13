using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyDemo.Entities
{
    public class OrderLine
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
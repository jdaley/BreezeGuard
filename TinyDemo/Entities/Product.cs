using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyDemo.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public int Stock { get; set; }
        public decimal WholesalePrice { get; set; }
    }
}
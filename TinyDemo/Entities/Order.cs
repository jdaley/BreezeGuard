using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyDemo.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public bool IsPaid { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public List<OrderLine> Lines { get; set; }
    }
}
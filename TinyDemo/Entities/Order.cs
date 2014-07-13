using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyDemo.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public bool IsPaid { get; set; }

        public List<OrderLine> Lines { get; set; }
    }
}
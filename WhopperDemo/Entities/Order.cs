using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhopperDemo.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public OrderStatus Status { get; set; }

        public List<OrderLine> Lines { get; set; }
    }
}
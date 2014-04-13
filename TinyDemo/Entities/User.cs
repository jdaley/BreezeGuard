using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyDemo.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
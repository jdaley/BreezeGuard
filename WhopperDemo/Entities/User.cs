using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhopperDemo.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public Role Role { get; set; }
    }
}
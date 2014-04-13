﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyDemo.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Order> Orders { get; set; }
        public List<User> Users { get; set; }
    }
}
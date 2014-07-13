using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WhopperDemo.Entities;

namespace WhopperDemo
{
    public class WhopperContext : DbContext
    {
        static WhopperContext()
        {
            Database.SetInitializer<WhopperContext>(new WhopperDatabaseInitializer());
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
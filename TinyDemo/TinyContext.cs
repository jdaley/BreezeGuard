using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TinyDemo.Entities;

namespace TinyDemo
{
    public class TinyContext : DbContext
    {
        static TinyContext()
        {
            Database.SetInitializer<TinyContext>(new TinyDatabaseInitializer());
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
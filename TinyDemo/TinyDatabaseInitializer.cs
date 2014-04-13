using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TinyDemo.Entities;

namespace TinyDemo
{
    public class TinyDatabaseInitializer : DropCreateDatabaseAlways<TinyContext>
    {
        protected override void Seed(TinyContext context)
        {
            context.Customers.Add(new Customer
            {
                Name = "Coconut Co"
            });

            context.Customers.Add(new Customer
            {
                Name = "Ink Inc"
            });

            context.Users.Add(new User
            {
                UserName = "bob",
                Password = "s3cr3t",
                CustomerId = 1
            });

            context.Users.Add(new User
            {
                UserName = "jane",
                Password = "p@ssword",
                CustomerId = 1
            });

            context.Users.Add(new User
            {
                UserName = "jill",
                Password = "u8Dj7VapL0esJhBv3",
                CustomerId = 2
            });
        }
    }
}
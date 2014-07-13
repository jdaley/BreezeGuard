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
            List<Product> products = new List<Product>
            {
                new Product
                {
                    Description = "Lemmings",
                    Price = 7.99M,
                    IsActive = true,
                    Stock = 10,
                    WholesalePrice = 4.70M
                },
                new Product
                {
                    Description = "Lemonade",
                    Price = 1.5M,
                    IsActive = true,
                    Stock = 84,
                    WholesalePrice = 0.30M
                },
                new Product
                {
                    Description = "Lemur",
                    Price = 300,
                    IsActive = false,
                    Stock = 1,
                    WholesalePrice = 240
                }
            };

            List<User> users = new List<User>
            {
                new User
                {
                    UserName = "bob",
                    Password = "s3cr3t"
                },
                new User
                {
                    UserName = "jane",
                    Password = "p@ssword"
                }
            };

            List<Order> orders = new List<Order>
            {
                new Order
                {
                    User = users[0],
                    IsPaid = false,
                    Lines = new List<OrderLine>
                    {
                        new OrderLine
                        {
                            Quantity = 2,
                            Product = products[1]
                        }
                    }
                },
                new Order
                {
                    User = users[1],
                    IsPaid = false,
                    Lines = new List<OrderLine>
                    {
                        new OrderLine
                        {
                            Quantity = 1,
                            Product = products[0]
                        }
                    }
                }
            };

            context.Products.AddRange(products);
            context.Users.AddRange(users);
            context.Orders.AddRange(orders);
        }
    }
}
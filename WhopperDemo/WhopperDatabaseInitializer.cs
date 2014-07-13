using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WhopperDemo.Entities;

namespace WhopperDemo
{
    public class WhopperDatabaseInitializer : DropCreateDatabaseAlways<WhopperContext>
    {
        protected override void Seed(WhopperContext context)
        {
            List<Product> products = new List<Product>
            {
                new Product
                {
                    Description = "Lemmings",
                    Price = 7.99M,
                    IsActive = true,
                    InventoryStockId = 1001
                },
                new Product
                {
                    Description = "Lemonade",
                    Price = 1.5M,
                    IsActive = true,
                    InventoryStockId = 1002
                },
                new Product
                {
                    Description = "Lemur",
                    Price = 300,
                    IsActive = false,
                    InventoryStockId = 1003
                }
            };

            List<User> users = new List<User>
            {
                new User
                {
                    UserName = "bob",
                    Password = "s3cr3t",
                    Role = Role.Customer
                },
                new User
                {
                    UserName = "jane",
                    Password = "p@ssword",
                    Role = Role.Customer
                },
                new User
                {
                    UserName = "steve",
                    Password = "123456",
                    Role = Role.Shipping
                },
                new User
                {
                    UserName = "jill",
                    Password = "u8Dj7VapL0esJhBv3",
                    Role = Role.Admin
                }
            };

            List<Order> orders = new List<Order>
            {
                new Order
                {
                    User = users[0],
                    Status = OrderStatus.Unpaid,
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
                    Status = OrderStatus.Unpaid,
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
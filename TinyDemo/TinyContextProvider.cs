using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using BreezeGuard;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TinyDemo.Controllers;
using TinyDemo.Entities;

namespace TinyDemo
{
    public class TinyContextProvider : BreezeGuardContextProvider<TinyContext>
    {
        protected override DbContext CreateMetadataContext()
        {
            return new TinyMetadataContext();
        }

        protected override void OnApiModelCreating(ApiModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasNavigation(c => c.Orders)
                .HasNavigation(c => c.Users);

            modelBuilder.Entity<Order>()
                .HasNavigation(o => o.Customer)
                .HasNavigation(o => o.Lines);

            modelBuilder.Entity<OrderLine>()
                .HasNavigation(ol => ol.Order);

            modelBuilder.Entity<User>()
                .HasNavigation(u => u.Customer)
                .Ignore(u => u.Password);
        }

        protected override List<ISaveHandler> CreateSaveHandlers()
        {
            return new List<ISaveHandler>
            {
                new OrderSaveHandler()
                {
                    Context = this.Context
                }
            };
        }
    }
}
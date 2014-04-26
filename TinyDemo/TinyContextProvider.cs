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

        protected override void OnModelCreating(ApiModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasResource<EntitiesController>(c => c.Customers());

            modelBuilder.Entity<Order>()
                .HasResource<EntitiesController>(c => c.Orders())
                .Allow(o => o.Lines);

            modelBuilder.Entity<OrderLine>()
                .HasResourceVia<Order>(ol => ol.Order);

            modelBuilder.Entity<User>()
                .HasResource<EntitiesController>(c => c.Users())
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
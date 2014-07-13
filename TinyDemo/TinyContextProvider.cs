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
            modelBuilder.Entity<Order>()
                .HasNavigation(o => o.Lines);

            modelBuilder.Entity<OrderLine>()
                .HasNavigation(ol => ol.Product);

            modelBuilder.Entity<Product>()
                .Ignore(p => p.Stock)
                .Ignore(p => p.WholesalePrice);
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
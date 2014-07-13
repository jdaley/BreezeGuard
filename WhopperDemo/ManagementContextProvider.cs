using BreezeGuard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WhopperDemo.Entities;

namespace WhopperDemo
{
    public class ManagementContextProvider : BreezeGuardContextProvider<WhopperContext>
    {
        protected override void OnApiModelCreating(ApiModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasNavigation(o => o.Lines);

            modelBuilder.Entity<OrderLine>()
                .HasNavigation(ol => ol.Product);

            modelBuilder.Entity<Product>();

            modelBuilder.Entity<User>()
                .Ignore(u => u.Password);
        }
    }
}
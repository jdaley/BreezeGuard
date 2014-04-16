using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using BreezeGuard;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
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
            modelBuilder.Entity<Customer>();
            modelBuilder.Entity<Order>();
            modelBuilder.Entity<OrderLine>();
            modelBuilder.Entity<User>().Ignore(u => u.Password);
        }
    }
}
using BreezeGuard;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TinyDemo.Entities;

namespace TinyDemo
{
    public class TinyMetadataContext : TinyContext
    {
        static TinyMetadataContext()
        {
            // Tell Entity Framework not to initialize a database for this context
            Database.SetInitializer<TinyMetadataContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            MetadataContextHelper.OnModelCreating(modelBuilder, new TinyContextProvider());
        }
    }
}
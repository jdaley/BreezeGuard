using Breeze.ContextProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public abstract class BreezeGuardContextProvider<TContext> : ContextProvider where TContext : DbContext
    {
        private static string jsonMetadata = null;
        private static object jsonMetadataLock = new object();

        private TContext context;
        private ApiModelBuilder model;

        protected BreezeGuardContextProvider()
        {
            this.context = null;
            this.model = ApiModelCache.Get(GetType(), OnModelCreating);
        }

        public TContext Context
        {
            get
            {
                if (this.context == null)
                {
                    this.context = CreateContext();
                }

                return this.context;
            }
        }

        public ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this.Context).ObjectContext; }
        }

        protected virtual TContext CreateContext()
        {
            return Activator.CreateInstance<TContext>();
        }

        protected virtual DbContext CreateMetadataContext()
        {
            return MetadataContextHelper.EmitMetadataContext<TContext>();
        }

        protected abstract void OnModelCreating(ApiModelBuilder modelBuilder);

        protected override string BuildJsonMetadata()
        {
            lock (jsonMetadataLock)
            {
                if (jsonMetadata == null)
                {
                    DbContext metadataContext = CreateMetadataContext();
                    jsonMetadata = MetadataContextHelper.GetMetadataFromContext(metadataContext);
                }

                return jsonMetadata;
            }
        }

        public override IDbConnection GetDbConnection()
        {
            return this.ObjectContext.Connection;
        }

        protected override void OpenDbConnection()
        {
            var ec = this.ObjectContext.Connection as EntityConnection;
            if (ec.State == ConnectionState.Closed) ec.Open();
        }

        protected override void CloseDbConnection()
        {
            var ec = this.ObjectContext.Connection as EntityConnection;
            ec.Close();
            ec.Dispose();
        }

        protected override void SaveChangesCore(SaveWorkState saveWorkState)
        {
            throw new NotImplementedException();
        }
    }
}

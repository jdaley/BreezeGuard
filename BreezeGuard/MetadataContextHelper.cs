using Breeze.ContextProvider.EF6;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public static class MetadataContextHelper
    {
        private static readonly PropertyInfo clrTypeProperty = typeof(EntityType).GetProperty("ClrType",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        public static DbContext EmitMetadataContext<TBaseContext>()
        {
            // TODO: Emit a metadata DbContext that extends TBaseContext, and return an instance of it
            throw new NotImplementedException();
        }

        public static void OnModelCreating<TDbContext>(DbModelBuilder modelBuilder,
            BreezeGuardContextProvider<TDbContext> contextProvider)
            where TDbContext : DbContext
        {
            ApiModelBuilder apiModel = ApiModelCache.Get(contextProvider.GetType());
            ObjectContext objectContext = ((IObjectContextAdapter)contextProvider.Context).ObjectContext;

            // Ignore all entities not part of the API model
            var ignoredTypes = GetEntityTypes(objectContext).Where(t => !apiModel.Entities.ContainsKey(t));
            modelBuilder.Ignore(ignoredTypes);

            // Explicitly ignored properties
            foreach (ApiEntityTypeConfiguration entityTypeConfiguration in apiModel.Entities.Values)
            {
                foreach (ApiIgnoredPropertyConfiguration ignoredPropertyConfiguration in entityTypeConfiguration.IgnoredProperties.Values)
                {
                    ignoredPropertyConfiguration.Apply(modelBuilder);
                }
            }
        }

        private static List<Type> GetEntityTypes(ObjectContext objectContext)
        {
            // Get Entity Framework's EntityTypes for the model, and read the internal ClrType property of each one
            var entityTypes = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace); // should this be OSpace or CSpace?
            return entityTypes.Select(e => (Type)clrTypeProperty.GetValue(e)).ToList();
        }

        public static string GetMetadataFromContext(DbContext context)
        {
            return EFContextProvider<DummyDbContext>.GetMetadataFromContext(context);
        }

        private class DummyDbContext : DbContext { }
    }
}

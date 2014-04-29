using Breeze.ContextProvider.EF6;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public static class MetadataContextHelper
    {
        private static readonly MethodInfo IgnoreMethodInfo =
            typeof(MetadataContextHelper).GetMethod("Ignore", BindingFlags.NonPublic | BindingFlags.Static);

        public static DbContext EmitMetadataContext<TBaseContext>()
        {
            // TODO: Emit a metadata DbContext that extends TBaseContext, and return an instance of it
            throw new NotImplementedException();
        }

        public static void OnModelCreating(DbModelBuilder modelBuilder, Type contextProviderType)
        {
            ApiModelBuilder apiModel = ModelCache.GetApiModel(contextProviderType);

            // Ignore all entities not part of the API model
            modelBuilder.Ignore(apiModel.GetIgnoredEntityTypes());

            // Explicitly ignored properties
            foreach (var entityTypeConfig in apiModel.GetEntityTypeConfigs())
            {
                foreach (PropertyInfo propertyInfo in entityTypeConfig.GetIgnoredProperties())
                {
                    // Call the Ignore method below with the type parameters for this PropertyInfo
                    IgnoreMethodInfo.MakeGenericMethod(entityTypeConfig.Type, propertyInfo.PropertyType).Invoke(
                        null, new object[] { modelBuilder, propertyInfo });
                }
            }
        }

        private static void Ignore<TEntityType, TProperty>(DbModelBuilder modelBuilder, PropertyInfo propertyInfo)
            where TEntityType : class
        {
            Expression<Func<TEntityType, TProperty>> propertyExpression =
                ExpressionHelper.ToPropertyExpression<TEntityType, TProperty>(propertyInfo);

            modelBuilder.Entity<TEntityType>().Ignore(propertyExpression);
        }

        public static string GetMetadataFromContext(DbContext context)
        {
            return EFContextProvider<DummyDbContext>.GetMetadataFromContext(context);
        }

        private class DummyDbContext : DbContext { }
    }
}

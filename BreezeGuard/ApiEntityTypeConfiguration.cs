using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    public abstract class ApiEntityTypeConfiguration
    {
        internal Type Type { get; private set; }
        private string contextName;
        protected internal Type ResourceType { get; protected set; }
        protected internal Delegate ResourceAccessor { get; protected set; }
        private Dictionary<PropertyInfo, ApiPropertyConfiguration> propertyConfigs;

        internal ApiEntityTypeConfiguration(Type type, string contextName, EntityType dbEntityType)
        {
            this.Type = type;
            this.contextName = contextName;
            this.ResourceType = null;
            this.ResourceAccessor = null;
            this.propertyConfigs = new Dictionary<PropertyInfo, ApiPropertyConfiguration>();

            foreach (EdmProperty dbProperty in dbEntityType.Properties)
            {
                PropertyInfo propertyInfo = type.GetProperty(dbProperty.Name);
                this.propertyConfigs.Add(propertyInfo, new ApiPropertyConfiguration(propertyInfo));
            }
        }

        internal ApiPropertyConfiguration Property(LambdaExpression propertyExpression)
        {
            ApiPropertyConfiguration propertyConfig;
            PropertyInfo propertyInfo = ExpressionHelper.ToPropertyInfo(propertyExpression);

            if (!this.propertyConfigs.TryGetValue(propertyInfo, out propertyConfig))
            {
                throw new Exception(string.Format("Context {0} does not have a scalar property {1} for entity {2}.",
                    this.contextName, propertyInfo.Name, this.Type));
            }

            return propertyConfig;
        }

        internal bool TryGetPropertyConfig(PropertyInfo propertyInfo, out ApiPropertyConfiguration propertyConfig)
        {
            return this.propertyConfigs.TryGetValue(propertyInfo, out propertyConfig);
        }

        internal IEnumerable<PropertyInfo> GetIgnoredProperties()
        {
            return this.propertyConfigs.Values.Where(pc => pc.Ignored).Select(pc => pc.PropertyInfo);
        }
    }

    public class ApiEntityTypeConfiguration<TEntityType> : ApiEntityTypeConfiguration where TEntityType : class
    {
        internal ApiEntityTypeConfiguration(string contextType, EntityType dbEntityType)
            : base(typeof(TEntityType), contextType, dbEntityType) { }

        public ApiEntityTypeConfiguration<TEntityType> HasResource<TResource>(
            Func<TResource, IQueryable<TEntityType>> resourceAccessor)
        {
            this.ResourceType = typeof(TResource);
            this.ResourceAccessor = resourceAccessor;
            return this;
        }

        public ApiEntityTypeConfiguration<TEntityType> HasResourceVia<TOwnerEntityType>(
            Expression<Func<TEntityType, TOwnerEntityType>> parentPropertyExpression)
        {
            return this;
        }

        public ApiEntityTypeConfiguration<TEntityType> Allow<TProperty>(
            Expression<Func<TEntityType, TProperty>> propertyExpression)
        {
            return this;
        }

        public ApiEntityTypeConfiguration<TEntityType> Ignore<TProperty>(
            Expression<Func<TEntityType, TProperty>> propertyExpression)
        {
            Property(propertyExpression).Ignored = true;
            return this;
        }
    }
}

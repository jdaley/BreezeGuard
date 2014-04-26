using System;
using System.Collections.Generic;
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
        internal Type ResourceType { get; set; }
        internal Delegate ResourceAccessor { get; set; }
        internal Dictionary<PropertyInfo, ApiIgnoredPropertyConfiguration> IgnoredProperties { get; private set; }

        internal ApiEntityTypeConfiguration(Type type)
        {
            this.Type = type;
            this.IgnoredProperties = new Dictionary<PropertyInfo, ApiIgnoredPropertyConfiguration>();
        }

        internal IQueryable AccessResource(object resource)
        {
            return (IQueryable)this.ResourceAccessor.DynamicInvoke(resource);
        }
    }

    public class ApiEntityTypeConfiguration<TEntityType> : ApiEntityTypeConfiguration where TEntityType : class
    {
        internal ApiEntityTypeConfiguration()
            : base(typeof(TEntityType)) { }

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
            PropertyInfo propertyInfo = ExpressionHelper.ToPropertyInfo(propertyExpression);
            this.IgnoredProperties[propertyInfo] = new ApiIgnoredPropertyConfiguration<TEntityType, TProperty>(
                propertyInfo, propertyExpression);
            return this;
        }
    }
}

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
        internal Dictionary<PropertyInfo, ApiIgnoredPropertyConfiguration> IgnoredProperties { get; private set; }

        internal ApiEntityTypeConfiguration(Type type)
        {
            this.Type = type;
            this.IgnoredProperties = new Dictionary<PropertyInfo, ApiIgnoredPropertyConfiguration>();
        }
    }

    public class ApiEntityTypeConfiguration<TEntityType> : ApiEntityTypeConfiguration where TEntityType : class
    {
        internal ApiEntityTypeConfiguration()
            : base(typeof(TEntityType)) { }

        public ApiEntityTypeConfiguration<TEntityType> HasResource<TController>(
            Func<TController, IQueryable<TEntityType>> resourceMethod)
        {
            return this;
        }

        public ApiEntityTypeConfiguration<TEntityType> HasResourceVia<TParentEntityType>(
            Expression<Func<TEntityType, TParentEntityType>> parentPropertyExpression)
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

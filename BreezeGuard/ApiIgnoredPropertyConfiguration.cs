using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    internal abstract class ApiIgnoredPropertyConfiguration
    {
        internal PropertyInfo PropertyInfo { get; private set; }

        internal ApiIgnoredPropertyConfiguration(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
        }

        internal abstract void Apply(DbModelBuilder dbModelBuilder);
    }

    internal class ApiIgnoredPropertyConfiguration<TEntityType, TProperty> : ApiIgnoredPropertyConfiguration
        where TEntityType : class
    {
        internal Expression<Func<TEntityType, TProperty>> PropertyExpression { get; private set; }

        internal ApiIgnoredPropertyConfiguration(PropertyInfo propertyInfo,
            Expression<Func<TEntityType, TProperty>> propertyExpression)
            : base(propertyInfo)
        {
            this.PropertyExpression = propertyExpression;
        }

        internal override void Apply(DbModelBuilder dbModelBuilder)
        {
            dbModelBuilder.Entity<TEntityType>().Ignore(this.PropertyExpression);
        }
    }
}

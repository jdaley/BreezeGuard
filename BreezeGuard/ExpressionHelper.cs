using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    internal static class ExpressionHelper
    {
        public static PropertyInfo ToPropertyInfo(LambdaExpression propertyExpression)
        {
            MemberExpression memberExp = (MemberExpression)propertyExpression.Body;
            return (PropertyInfo)memberExp.Member;
        }
    }
}

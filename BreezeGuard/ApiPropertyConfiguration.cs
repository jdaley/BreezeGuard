using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BreezeGuard
{
    internal class ApiPropertyConfiguration
    {
        internal PropertyInfo PropertyInfo { get; private set; }
        internal bool Ignored { get; set; }

        internal ApiPropertyConfiguration(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.Ignored = false;
        }
    }
}

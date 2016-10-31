using Breeze.WebApi2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BreezeGuard
{
    public class BreezeGuardControllerAttribute : BreezeControllerAttribute, IControllerConfiguration
    {
        public Type ContextProviderType { get; private set; }

        public BreezeGuardControllerAttribute(Type contextProviderType)
        {
            // TODO: contextProviderType must be a Type that extends BreezeGuardContextProvider

            this.ContextProviderType = contextProviderType;
        }

        protected override IFilterProvider GetQueryableFilterProvider(EnableBreezeQueryAttribute defaultFilter)
        {
            return base.GetQueryableFilterProvider(new BreezeGuardQueryableAttribute(this.ContextProviderType));
        }

        protected override MediaTypeFormatter GetJsonFormatter()
        {
            JsonMediaTypeFormatter formatter = JsonFormatter.Create();

            if (formatter.SerializerSettings.ContractResolver != null)
            {
                throw new Exception("BreezeGuard does not support custom JSON ContractResolvers.");
            }

            // TODO: Is formatter.SerializerSettings and object that is shared across all Breeze formatters? If so,
            // we may have to clone it here.

            formatter.SerializerSettings.ContractResolver =
                new BreezeGuardContractResolver(this.ContextProviderType);

            return formatter;
        }
    }
}

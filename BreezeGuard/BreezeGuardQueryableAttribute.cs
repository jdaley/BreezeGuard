using Breeze.WebApi2;
using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace BreezeGuard
{
    public class BreezeGuardQueryableAttribute : BreezeQueryableAttribute
    {
        public Type ContextProviderType { get; private set; }

        public BreezeGuardQueryableAttribute(Type contextProviderType)
        {
            this.ContextProviderType = contextProviderType;
        }

        public override IEdmModel GetModel(Type elementClrType, HttpRequestMessage request,
            HttpActionDescriptor actionDescriptor)
        {
            return ModelCache.GetODataModel(this.ContextProviderType);
        }
    }
}

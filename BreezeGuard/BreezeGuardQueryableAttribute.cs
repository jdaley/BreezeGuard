using Breeze.WebApi2;
using Microsoft.Data.Edm;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace BreezeGuard
{
    public class BreezeGuardQueryableAttribute : EnableBreezeQueryAttribute
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

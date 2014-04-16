using Breeze.WebApi2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace BreezeGuard
{
    public class BreezeGuardControllerAttribute : Attribute, IControllerConfiguration
    {
        public Type ContextProviderType { get; private set; }

        public BreezeGuardControllerAttribute(Type contextProviderType)
        {
            // TODO: contextProviderType must be a Type that extends BreezeGuardContextProvider

            this.ContextProviderType = contextProviderType;
        }

        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.Clear();
            controllerSettings.Formatters.Add(JsonFormatter.Create());
        }
    }
}

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
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.Clear();
            controllerSettings.Formatters.Add(JsonFormatter.Create());
        }
    }
}

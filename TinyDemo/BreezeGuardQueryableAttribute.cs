using Breeze.WebApi2;
using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Query;
using TinyDemo.Entities;

namespace TinyDemo
{
    public class BreezeGuardQueryableAttribute : BreezeQueryableAttribute
    {
        public override IEdmModel GetModel(Type elementClrType, HttpRequestMessage request, HttpActionDescriptor actionDescriptor)
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.Entity<Customer>();
            builder.Entity<Order>();
            builder.Entity<User>().Ignore(u => u.Password);
            return builder.GetEdmModel();
        }
    }
}
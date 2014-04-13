using Breeze.ContextProvider;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TinyDemo.Entities;

namespace TinyDemo.Controllers
{
    [BreezeController]
    public class EntitiesController : ApiController
    {
        private TinyContextProvider provider = new TinyContextProvider();

        [HttpGet]
        public string Metadata()
        {
            return this.provider.Metadata();
        }

        [HttpGet]
        public IQueryable<Customer> Customers()
        {
            return this.provider.Context.Customers;
        }

        [HttpGet]
        public IQueryable<Order> Orders()
        {
            return this.provider.Context.Orders;
        }

        [HttpGet]
        public IQueryable<User> Users()
        {
            return this.provider.Context.Users;
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return provider.SaveChanges(saveBundle);
        }
    }
}
using Breeze.ContextProvider;
using Breeze.WebApi2;
using BreezeGuard;
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
    [BreezeGuardController(typeof(TinyContextProvider))]
    public class EntitiesController : ApiController
    {
        private TinyContextProvider provider;
        private User user;

        public EntitiesController()
        {
            this.provider = new TinyContextProvider();

            int userId = 1; // get the logged in user ID, perhaps from a session cookie
            this.user = this.provider.Context.Users.Find(userId);
        }

        [HttpGet]
        public string Metadata()
        {
            return this.provider.Metadata();
        }

        [HttpGet]
        public IQueryable<Customer> Customers()
        {
            return this.provider.Context.Customers.Where(c => c.Id == this.user.CustomerId);
        }

        [HttpGet]
        public IQueryable<Order> Orders()
        {
            return this.provider.Context.Orders.Where(o => o.CustomerId == this.user.CustomerId);
        }

        [HttpGet]
        public IQueryable<User> Users()
        {
            return this.provider.Context.Users.Where(u => u.CustomerId == this.user.CustomerId);
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return provider.SaveChanges(saveBundle);
        }
    }
}
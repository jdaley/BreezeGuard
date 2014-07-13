using Breeze.ContextProvider;
using Breeze.WebApi2;
using BreezeGuard;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TinyDemo.Entities;
using TinyDemo.Models;

namespace TinyDemo.Controllers
{
    [BreezeGuardController(typeof(TinyContextProvider))]
    public class TinyDemoController : ApiController
    {
        private TinyContextProvider provider;
        private User user;

        public TinyDemoController()
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
        public IQueryable<Order> Orders()
        {
            return this.provider.Context.Orders.Where(o => o.UserId == this.user.Id);
        }

        [HttpGet]
        public IQueryable<Product> Products()
        {
            return this.provider.Context.Products.Where(p => p.IsActive);
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            this.provider.AddResource(this);
            return this.provider.SaveChanges(saveBundle);
        }

        [HttpPost]
        public HttpResponseMessage ProcessPayment(PaymentRequest paymentRequest)
        {
            // Orders() is already filtered by UserId, so it will protect against attempts to
            // pass in an OrderId for an order belonging to a different user.
            Order order = Orders().Include(o => o.Lines).FirstOrDefault(o => o.Id == paymentRequest.OrderId);

            if (order == null)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Order does not exist.")
                };
            }

            if (order.IsPaid)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Order is already paid.")
                };
            }

            // Process the credit card payment
            // (not really)
            
            // Mark the order as paid
            order.IsPaid = true;
            this.provider.Context.SaveChanges();

            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("Approved")
            };
        }
    }
}
using BreezeGuard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WhopperDemo.Entities;

namespace WhopperDemo.Controllers
{
    [BreezeGuardController(typeof(OrderingContextProvider))]
    public class OrderingController : ApiController
    {
        private WhopperContext context;

        public IQueryable<Order> Orders()
        {
            return this.context.Orders.Where(o => o.UserId == 1);
        }
    }
}

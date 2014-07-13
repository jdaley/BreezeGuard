using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhopperDemo.Entities
{
    public enum OrderStatus
    {
        /// <summary>
        /// Customer has not yet paid for the order.
        /// </summary>
        Unpaid,

        /// <summary>
        /// Customer has paid for the order, now waiting for shipping department to process.
        /// </summary>
        Paid,

        /// <summary>
        /// Shipping department has sent the order.
        /// </summary>
        Shipped
    }
}
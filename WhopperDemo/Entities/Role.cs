using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhopperDemo.Entities
{
    public enum Role
    {
        /// <summary>
        /// View products, create orders, modify unpaid orders, pay for orders.
        /// </summary>
        Customer,

        /// <summary>
        /// View orders for all customers, mark orders as shipped, view stock levels.
        /// </summary>
        Shipping,

        /// <summary>
        /// Manage products, users and orders, including access to modify paid orders.
        /// </summary>
        Admin
    }
}
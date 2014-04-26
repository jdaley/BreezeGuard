using BreezeGuard;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TinyDemo.Entities;

namespace TinyDemo
{
    public class OrderSaveHandler : SaveHandler<Order, TinyContext>
    {
        public override void Save(SaveModel<Order> saveModel)
        {
            Order order = saveModel.Entity;

            if (saveModel.IsAdded)
            {
                order = new Order
                {
                    IsPaid = false,
                    CustomerId = 1,
                    Lines = new List<OrderLine>()
                };

                this.Context.Orders.Add(order);
                saveModel.Entity = order;
            }
            else
            {
                if (order.IsPaid)
                {
                    throw new Exception("Cannot modify an order after it has been paid.");
                }
            }

            foreach (var orderLineInfo in saveModel.Get<OrderLine>())
            {
                OrderLine orderLine = orderLineInfo.Entity;

                if (orderLineInfo.IsAdded)
                {
                    orderLine = new OrderLine();
                    order.Lines.Add(orderLine);
                    orderLineInfo.Entity = orderLine;
                }

                orderLineInfo.Apply(ol => ol.Description);
                orderLineInfo.Apply(ol => ol.Price);

                if (orderLineInfo.IsDeleted)
                {
                    this.Context.Entry(orderLine).State = EntityState.Deleted;
                }
            }

            if (saveModel.IsDeleted)
            {
                this.Context.Entry(order).State = EntityState.Deleted;
            }
        }
    }
}
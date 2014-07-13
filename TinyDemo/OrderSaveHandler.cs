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
        public override void Save(SaveEntity<Order> saveEntity)
        {
            Order order = saveEntity.Entity;

            if (saveEntity.IsAdded)
            {
                order = new Order
                {
                    UserId = 1,
                    IsPaid = false,
                    Lines = new List<OrderLine>()
                };

                this.Context.Orders.Add(order);
                saveEntity.Entity = order;
            }
            else
            {
                if (order.IsPaid)
                {
                    throw new Exception("Cannot modify an order after it has been paid.");
                }
            }

            foreach (var orderLineInfo in saveEntity.Get<OrderLine>())
            {
                OrderLine orderLine = orderLineInfo.Entity;

                if (orderLineInfo.IsAdded)
                {
                    orderLine = new OrderLine();
                    order.Lines.Add(orderLine);
                    orderLineInfo.Entity = orderLine;
                }

                orderLineInfo.Apply(ol => ol.Quantity);
                orderLineInfo.Apply(ol => ol.ProductId);

                if (orderLineInfo.IsDeleted)
                {
                    this.Context.Entry(orderLine).State = EntityState.Deleted;
                }
            }

            if (saveEntity.IsDeleted)
            {
                this.Context.Entry(order).State = EntityState.Deleted;
            }
        }
    }
}
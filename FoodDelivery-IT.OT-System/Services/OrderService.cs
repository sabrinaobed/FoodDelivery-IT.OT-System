using FoodDelivery_IT.OT_System.Data;
using FoodDelivery_IT.OT_System.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodDelivery_IT.OT_System.Interfaces;
using FoodDelivery_IT.OT_System.Models;

namespace FoodDelivery_IT.OT_System.Services
{
    public class OrderService  : IOrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }

        //Get all orders
        public IEnumerable<OrderModel> GetAll() =>
         _db.Orders.AsNoTracking()
                   .OrderByDescending(o => o.OrderId)
                   .ToList();

        //Create order for customer
        public OrderModel CreateForCustomer(string customerName, string dishName, int quantity, string createdByStaffEmail)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required.", nameof(customerName));
            if (string.IsNullOrWhiteSpace(dishName))
                throw new ArgumentException("Dish name is required.", nameof(dishName));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be a positive number.", nameof(quantity));
            if (string.IsNullOrWhiteSpace(createdByStaffEmail))
                throw new ArgumentException("Creator email is required.", nameof(createdByStaffEmail));

            var order = new OrderModel
            {
                CustomerName = customerName.Trim(),
                DishName = dishName.Trim(),
                Quantity = quantity,
                CreatedBy = createdByStaffEmail,
                OrderStatus = OrderStatus.Placed,
                SentToOT = false
            };

            _db.Orders.Add(order);
            _db.SaveChanges();
            return order;
        }

        //Mark order as Processing
        public void MarkProcessing(int orderId) => UpdateStatus(orderId, OrderStatus.Processing);

        //mark order as sent to OT
        public void MarkSent(int orderId) => UpdateStatus(orderId, OrderStatus.Sent);
        public void MarkSentToOT(int orderId)
        {
            var stub = new OrderModel { OrderId = orderId, SentToOT = true };
            _db.Attach(stub);
            _db.Entry(stub).Property(o => o.SentToOT).IsModified = true;
            _db.SaveChanges();
        }


        //Helepr method
        private void UpdateStatus(int orderId, OrderStatus status)
        {
            var entity = _db.Orders.SingleOrDefault(o => o.OrderId == orderId)
                         ?? throw new KeyNotFoundException($"Order {orderId} not found.");
            entity.OrderStatus = status;
            _db.SaveChanges();
        }

    }
}

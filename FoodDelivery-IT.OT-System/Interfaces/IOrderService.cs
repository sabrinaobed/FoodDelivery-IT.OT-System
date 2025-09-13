using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodDelivery_IT.OT_System.Models;

namespace FoodDelivery_IT.OT_System.Interfaces
{
    public interface IOrderService
    {
        /// <summary>List all orders (restaurant staff overview, newest first).</summary>
        IEnumerable<OrderModel> GetAll();

        /// <summary>Create a new order on behalf of a customer.</summary>
        OrderModel CreateForCustomer(string customerName, string dishName, int quantity, string createdByStaffEmail);

        /// <summary>Mark order as Processing (used by Integration Agent after POST to OT).</summary>
        void MarkProcessing(int orderId);

        /// <summary>Mark order as Sent (used by Integration Agent when OT says “ready/handed to rider”).</summary>
        void MarkSent(int orderId);

        /// <summary>Flip SentToOT to true (after a successful send to OT).</summary>
        void MarkSentToOT(int orderId);
    }
}

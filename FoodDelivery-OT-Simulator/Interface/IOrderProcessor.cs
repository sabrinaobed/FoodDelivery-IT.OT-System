using FoodDelivery_OT_Simulator.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FoodDelivery_OT_Simulator.Interface
{
    public interface IOrderProcessor
    {
        ReceiptDTO ReceiveOrder(OrderDTO order);
        public (bool found, string status) GetStatus(int orderId);
    }
}

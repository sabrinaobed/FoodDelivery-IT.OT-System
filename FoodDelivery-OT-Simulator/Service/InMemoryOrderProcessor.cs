using FoodDelivery_OT_Simulator.DTOs;
using FoodDelivery_OT_Simulator.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_OT_Simulator.Service
{
    public class InMemoryOrderProcessor :IOrderProcessor
    {
        private readonly ConcurrentDictionary<int, string> _statuses = new();

        public ReceiptDTO ReceiveOrder(OrderDTO order)
        {
            _statuses[order.OrderId ] = "Processing";
            Console.WriteLine($"[Kitchen/OT] Received order #{order.OrderId} for {order.CustomerName}: {order.DishName} x{order.Quantity}");

            _ = Task.Run(async () =>
            {
                await Task.Delay(2000);
                _statuses[order.OrderId] = "Sent";
                Console.WriteLine($"[GivenToRider/OT] Order #{order.OrderId} -> Sent (given to rider).");
            });

            return new ReceiptDTO(order.OrderId, "Processing", DateTime.UtcNow);
        }

        public (bool found, string status) GetStatus(int orderId)
            => _statuses.TryGetValue(orderId, out var s) ? (true, s) : (false, "");
    }
    
}

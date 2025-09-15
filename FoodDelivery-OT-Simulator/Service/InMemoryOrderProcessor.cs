using FoodDelivery_OT_Simulator.DTOs;
using FoodDelivery_OT_Simulator.Interface;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FoodDelivery_OT_Simulator.Service
{
    public class InMemoryOrderProcessor : IOrderProcessor
    {
        private readonly ConcurrentDictionary<int, string> _statuses = new();

        public ReceiptDTO ReceiveOrder(OrderDTO order)
        {
            // 1) Show that the kitchen has received the order (human-friendly)
            Console.WriteLine($"[Kitchen] Order #{order.OrderId} Placed: {order.DishName} x {order.Quantity} for {order.CustomerName}");

            // 2) Mark as Processing and log it
            _statuses[order.OrderId] = "Processing";
            Console.WriteLine($"[Kitchen] Order #{order.OrderId}: Processing");

            // 3) Simulate preparation: after 2 seconds, mark as Sent to rider
            _ = Task.Run(async () =>
            {
                await Task.Delay(2000);
                _statuses[order.OrderId] = "Sent";
                Console.WriteLine($"[Kitchen] Order #{order.OrderId}: Sent to rider");
            });

            // Immediate receipt back to caller
            return new ReceiptDTO(order.OrderId, "Processing", DateTime.UtcNow);
        }

        public (bool found, string status) GetStatus(int orderId)
            => _statuses.TryGetValue(orderId, out var s) ? (true, s) : (false, "");
    }
}

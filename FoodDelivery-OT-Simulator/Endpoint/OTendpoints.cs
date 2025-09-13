using FoodDelivery_OT_Simulator.DTOs;
using FoodDelivery_OT_Simulator.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FoodDelivery_OT_Simulator.Endpoint
{
    public static class OTendpoints
    {
        public static IEndpointRouteBuilder MapOtEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/orders", (OrderDTO order, IOrderProcessor machine) =>
            {
                var receipt = machine.ReceiveOrder(order);
                return Results.Ok(receipt);
            });

            app.MapGet("/api/orders/{id:int}/status", (int id, IOrderProcessor machine) =>
            {
                var (found, status) = machine.GetStatus(id);
                return found
                    ? Results.Ok(new { OrderId = id, Status = status })
                    : Results.NotFound(new { Message = "Order not found in OT." });
            });

            return app;
        }
    }
}

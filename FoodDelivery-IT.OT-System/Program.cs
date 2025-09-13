using FoodDelivery_IT.OT_System.Data;
using FoodDelivery_IT.OT_System.Interfaces;
using FoodDelivery_IT.OT_System.Menu;
using FoodDelivery_IT.OT_System.Services;
using Microsoft.Extensions.DependencyInjection;
using FoodDelivery_IT.OT_System.Data;
using FoodDelivery_IT.OT_System.Menu;

namespace FoodDelivery_IT.OT_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataInitialization.Seed();

            var services = new ServiceCollection()
                .AddDbContext<AppDbContext>()
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IOrderService, OrderService>()
                .BuildServiceProvider();

            // Resolve dependencies
            var auth = services.GetRequiredService<IAuthService>();
            var orders = services.GetRequiredService<IOrderService>();

            // Run the menu
            Meny menu = new Meny((AuthService)auth, (OrderService)orders);
        }
    }
}

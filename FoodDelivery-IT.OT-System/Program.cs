using FoodDelivery_IT.OT_System.Data;
using FoodDelivery_IT.OT_System.Interfaces;
using FoodDelivery_IT.OT_System.Services;
using FoodDelivery_IT.OT_System.UI;     // <-- add this
using Microsoft.Extensions.DependencyInjection;

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

            var auth = services.GetRequiredService<IAuthService>();
            var orders = services.GetRequiredService<IOrderService>();

            var menu = new Menu(auth, orders);
            menu.Start();
        }
    }
}

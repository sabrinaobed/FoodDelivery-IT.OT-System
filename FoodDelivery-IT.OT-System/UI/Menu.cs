using FoodDelivery_IT.OT_System.Interfaces;
using FoodDelivery_IT.OT_System.Models;

namespace FoodDelivery_IT.OT_System.UI
{
    public class Menu
    {
        private readonly IAuthService _auth;
        private readonly IOrderService _orders;

        public Menu(IAuthService auth, IOrderService orders)
        {
            _auth = auth;
            _orders = orders;
        }

        public void Start()
        {
            Console.WriteLine("======= Login to Food Delivery IT System =======");
            Console.Write("Email: ");
            var email = (Console.ReadLine() ?? "").Trim();

            Console.Write("Password: ");
            var password = (Console.ReadLine() ?? "").Trim();

            var currentUser = _auth.Login(email, password);
            if (currentUser is null)
            {
                Console.WriteLine("\nInvalid credentials. Exiting...");
                return;
            }

            Console.WriteLine($"\nWelcome {currentUser.FullName}! (Role: {currentUser.Role})");

            while (true)
            {
                Console.WriteLine("\n==== Restaurant Staff Portal ====");
                Console.WriteLine("1) List Orders");
                Console.WriteLine("2) Create Order (on behalf of customer)");
                Console.WriteLine("0) Exit");
                Console.Write("Choose: ");

                var choice = (Console.ReadLine() ?? "").Trim();
                if (choice is "0" or "") break;

                switch (choice)
                {
                    case "1":
                        ShowOrders();
                        break;
                    case "2":
                        CreateOrder(currentUser);
                        break;
                    default:
                        Console.WriteLine("Unknown choice.");
                        break;
                }
            }
        }

        private void ShowOrders()
        {
            var all = _orders.GetAll();
            if (!all.Any())
            {
                Console.WriteLine("No orders placed yet.");
                return;
            }

            Console.WriteLine("#  Customer        Dish            Qty  Status       SentToOT  CreatedBy              CreatedAt");
            foreach (var o in all)
            {
                Console.WriteLine($"{o.OrderId,-3}{o.CustomerName,-15}{o.DishName,-16}{o.Quantity,-4}{o.OrderStatus,-12}{o.SentToOT,-9}{o.CreatedBy,-22}{o.CreatedAt:yyyy-MM-dd HH:mm}");
            }
        }

        private void CreateOrder(UserModel currentUser)
        {
            Console.Write("Customer Name: ");
            var customerName = (Console.ReadLine() ?? "").Trim();

            Console.Write("Dish Name: ");
            var dishName = (Console.ReadLine() ?? "").Trim();

            Console.Write("Quantity: ");
            var qtyText = Console.ReadLine();
            if (!int.TryParse(qtyText, out var qty) || qty <= 0)
            {
                Console.WriteLine("Quantity must be a positive number.");
                return;
            }

            try
            {
                var created = _orders.CreateForCustomer(customerName, dishName, qty, currentUser.Email);
                Console.WriteLine($"Created Order #{created.OrderId}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}


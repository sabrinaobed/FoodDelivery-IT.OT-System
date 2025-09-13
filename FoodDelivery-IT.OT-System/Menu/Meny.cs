using FoodDelivery_IT.OT_System.Models;
using FoodDelivery_IT.OT_System.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_IT.OT_System.Menu
{
    public class Meny
    {
        private readonly AuthService _authService;
        private readonly OrderService _orderService;


        public Meny(AuthService authService, OrderService orderSrvice)
        {
            _authService = authService;
            _orderService = orderSrvice;
        }


        public void Start()
        {
            Console.WriteLine("=======Login To Food Delivery IT System========");
            Console.Write("Email: ");
            var email = Console.ReadLine().Trim() ?? "";

            Console.Write("Password: ");
            var password = Console.ReadLine().Trim() ?? "";

            var currentUser = _authService.Login(email, password);
            if(currentUser is null)
            {
                Console.WriteLine("\n Invalid credentials.Exiting.....");
                return;
            }

            Console.WriteLine($"\nWelcome {currentUser.FullName} to Food Delivery IT system Staff Portal!");


            //Main menu loop
            while(true)
            {
                Console.WriteLine("====Food Delivery IT System Resturant Staff Portal====");
                Console.WriteLine($"Welcome! Youa are Logged in as: {currentUser.Email} ({currentUser.Role})");
                Console.WriteLine("1- See List of all Orders");
                Console.WriteLine("2- Create/Place an order (on behalf of a customer)");
                Console.WriteLine("0- Exit");
                Console.WriteLine("Choose: ");

                var choice = Console.ReadLine().Trim();
                if (choice is "0" or null)
                    break;

                switch(choice)
                {
                    case "1":
                        ShowOrders();
                        break;
                    case "2":
                        CreateOrder(currentUser);
                        break;
                    default:
                        Console.WriteLine("Unkown choice.\n");
                        break;
                }


               
            }


        }


        //Helpers methods
        private void ShowOrders()
        {
            var all = _orderService.GetAll();
            if(!all.Any())
            {
                Console.WriteLine("No orders placed yet.\n");
                return;
            }

            Console.WriteLine("#  CustomerName    DishName     Qty    OrderStatus     SentToOT   CreatedBy   CreatedAt");
            foreach(var o in all)
            {
                Console.WriteLine($"{o.OrderId, -3} {o.CustomerName,-12} {o.DishName,-14} {o.Quantity, -3} {o.OrderStatus,-11}{o.SentToOT,-8}{o.CreatedBy,-18} {o.CreatedAt:yyyy-MM-dd HH:mm}");
            }
            Console.WriteLine();

        }


        private void CreateOrder(UserModel currentUser)
        {
            Console.Write("Customer Name: ");
            var customerName = Console.ReadLine().Trim() ?? "";

            Console.Write("Dish Name: ");
            var dishName = Console.ReadLine().Trim() ?? "";

            Console.Write("Quantity: ");
            var quantityText = Console.ReadLine();
            if(!int.TryParse(quantityText,out var qty))
            {
                Console.WriteLine("Quantity must be in number.\n");
                return;
            }
            try
            {
                var created = _orderService.CreateForCustomer(customerName!, dishName!, qty, currentUser.Email);
                Console.WriteLine($"Created Order #{created.OrderId}.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }


        }
    }
}

using FoodDelivery_IT.OT_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_IT.OT_System.Data
{
    public class DataInitiakization
    {
        public static void Seed()//this method seeds initial data into the database
        {
            using var db = new AppDbContext(); //creating a new databse context instance inside using block and using var ensures that once we are done the context will be disposed


            db.Database.EnsureCreated(); // ensures database exists and creates it if it does not


            if (!db.Users.Any())//this will be executed if theres no user in the database
            {
               var staffUser = new UserModel //create a new user object to represent the default staff user
               {
                    FullName = "Restaurant Staff",
                    Email = "staff@restaurant.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff123!"),
                    Role = "Staff"
               };
                //add the staff user to the user databse table
                db.Users.Add(staffUser);

                //save the changes to the database
                db.SaveChanges();

                //print the message to the console
                Console.WriteLine("Seeded staff: staff@restaurant.com / Staff123!");
            }
        }
    }
}

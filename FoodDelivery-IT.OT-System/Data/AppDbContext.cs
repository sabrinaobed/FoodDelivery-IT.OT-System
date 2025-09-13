using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodDelivery_IT.OT_System.Models;
using FoodDelivery_IT.OT_System.Enum;

namespace FoodDelivery_IT.OT_System.Data
{
    public class AppDbContext : DbContext //bridge betwween models and SQL Server
    {
        //creates tables
        public DbSet<UserModel> Users => Set<UserModel>();
        public DbSet<OrderModel> Orders => Set<OrderModel>();


        //Design-time constructor, migrations use this
        public AppDbContext() { }

        //Optional DI constructor , its not reuiqred for console
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //for configuration 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var cs = config.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(cs);
            }
        }

        //for model configuration like while creating models
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //store order status enum as int
            modelBuilder.Entity<OrderModel>()
            .Property(o => o.OrderStatus)
            .HasConversion<int>();

            //unique email for login

            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);


        }
    }
}

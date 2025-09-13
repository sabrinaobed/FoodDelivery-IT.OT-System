using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_IT.OT_System.Models
{
    public class OrderModel
    {
        [Key]
        public int OrderId { get; set; }

        [Required, MaxLength(100)]
        public string CustomerName { get; set; } = "";  // who the order is for

        [Required, MaxLength(100)]
        public string DishName { get; set; } = "";      // what was ordered

        [Range(1, 100)]
        public int Quantity { get; set; }

        //public OrderStatus Status { get; set; } = OrderStatus.Placed;

        public bool SentToOT { get; set; } = false; // has it been sent to OT system

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(255)]
        public string CreatedBy { get; set; } = ""; // staff who created order
    }
}

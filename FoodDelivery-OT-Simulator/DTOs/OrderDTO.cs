using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_OT_Simulator.DTOs
{
    public record OrderDTO
    (
    int OrderId,        // Unique order id assigned by IT
    string CustomerName,// Who the food is for (shown in OT logs)
    string DishName,    // What to prepare
    int Quantity        // How many portio
    );
}

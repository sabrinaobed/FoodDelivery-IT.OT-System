using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_IT.OT_System.Enum
{
    public enum OrderStatus
    {
        Placed = 0,      // Order has been placed by staff on behalf of customer
        Processing = 1,  // Order is being cooked/prepared
        Sent = 2         // Order is ready and handed to rider
    }
}

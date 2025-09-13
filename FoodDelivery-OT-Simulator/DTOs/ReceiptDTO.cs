using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_OT_Simulator.DTOs
{
    public record ReceiptDTO
    (
      int OrderId,           // echoes back the order id
    string Status,         // "Processing" (immediate response)
    DateTime ReceivedAt    // server timestamp (UTC)
    );


}

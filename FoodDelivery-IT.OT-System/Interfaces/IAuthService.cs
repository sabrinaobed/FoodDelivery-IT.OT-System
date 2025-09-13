using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodDelivery_IT.OT_System.Models;

namespace FoodDelivery_IT.OT_System.Interfaces
{
    public interface IAuthService
    {
        //Returns the logged-in user or null if credentials are invalid.</summary>
        UserModel? Login(string email, string password);
    }
}

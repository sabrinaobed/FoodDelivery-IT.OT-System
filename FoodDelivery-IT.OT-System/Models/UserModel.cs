using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_IT.OT_System.Models
{
    public class UserModel
    {
       
            [Key]
            public int UserId { get; set; }   // primary key

            [Required, MaxLength(100)]
            public string FullName { get; set; } = "";

            [Required, EmailAddress, MaxLength(255)]
            public string Email { get; set; } = "";

            [Required, MaxLength(255)]
            public string PasswordHash { get; set; } = ""; // store BCrypt hash

            [Required, MaxLength(50)]
            public string Role { get; set; } = "Staff"; // default role = Staff
        
    }
}

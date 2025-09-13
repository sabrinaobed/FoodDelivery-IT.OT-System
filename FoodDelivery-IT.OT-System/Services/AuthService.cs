using FoodDelivery_IT.OT_System.Data;
using FoodDelivery_IT.OT_System.Interfaces;
using FoodDelivery_IT.OT_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_IT.OT_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;

        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public UserModel? Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = _db.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
                return null;

            var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return ok ? user : null;
        }

    }
}

using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {

        public UserRepository(AppDbContext db) : base(db)
        {

        }
        public bool CreateUser(User user, string Role)
        {
            try
            {

                User data = _db.Users.Where(u => u.Email == user.Email).FirstOrDefault();
                if (data == null)
                {
                    Role role = _db.Roles.Where(r => r.Name == Role).FirstOrDefault();
                    if (role != null)
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                        user.Roles.Add(role);

                        _db.Users.Add(user);
                        _db.SaveChanges();
                        return true;
                    }
                }
                return false;

            }catch(Exception ex)
            {
                return false;
            }
            return false;
        }

        public UserModel ValidateUser(string Email, string Password)
        {
            User user = _db.Users.Include(u => u.Roles).Where(u=>u.Email== Email).FirstOrDefault();
            if(user != null)
            {
                bool isVerified = BCrypt.Net.BCrypt.Verify(Password,user.Password);
                if(isVerified)
                {
                    UserModel model = new UserModel()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Roles = user.Roles.Select(r => r.Name).ToArray(),
                    };
                    return model;
                }
            }
            return null;
        }

       
    }
}

using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Interfaces
{
    public interface IAuthService
    {
        bool CreateUser(User user, string Role);
        UserModel ValidateUser(string Email, string Password);
    }
}

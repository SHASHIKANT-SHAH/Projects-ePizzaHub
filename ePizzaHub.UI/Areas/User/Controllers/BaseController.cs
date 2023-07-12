using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Claims;
using ePizzaHub.UI.Helpers;

namespace ePizzaHub.UI.Areas.User.Controllers
{
    [CustomAuthorize(Roles = "User")]
    [Area("User")]
    public class BaseController : Controller
    {
        public UserModel CurrentUser
        {
            get
            {
                if (User.Claims.Count() > 0)
                {
                    string userData = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value;
                    var user = JsonSerializer.Deserialize<UserModel>(userData);
                    return user;
                }
                return null;
            }
        }
    }
}

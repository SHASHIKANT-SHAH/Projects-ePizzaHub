using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ePizzaHub.UI.Controllers
{
    public class AccountController : Controller
    {
        IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        void GenerateTicket(UserModel user)
        {
            string strData = JsonSerializer.Serialize(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.UserData,strData),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,string.Join(",",user.Roles))
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            });
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model, string returnUrl)
        {
            UserModel user = _authService.ValidateUser(model.Email, model.Password);
            if (user != null)
            {
                GenerateTicket(user);
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else if (user.Roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home", new {area = "Admin"});
                }else if (user.Roles.Contains("User"))
                {
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
            }
            else
            {
                ViewBag.Message = "Username or Password doesn't exits";
            }
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("Login","Account");
        }

        public IActionResult UnAuthorize()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(UserViewModel model)
        {
            //UserModel data = _authService.ValidateUser(model.Email,model.Password);

            //if (data != null)
            //{
                if (ModelState.IsValid)
                {
                    User user = new User
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                        PhoneNumber = model.PhoneNumber,
                        CreatedDate = DateTime.Now,
                    };
                    bool result = _authService.CreateUser(user, "User");
                    if (result)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ViewBag.Message = "Use Different Email-ID";
                    }

                }
           
           // }
                return View();
        }

        
    }
}

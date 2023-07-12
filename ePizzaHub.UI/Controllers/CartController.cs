using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ePizzaHub.UI.Controllers
{
    public class CartController : BaseController
    {
        ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService= cartService;
        }

        Guid CartId
        {
            get
            {
                Guid Id;
                string CId = Request.Cookies["CId"];
                if (String.IsNullOrEmpty(CId))
                {
                    Id = Guid.NewGuid();
                    Response.Cookies.Append("CId", Id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(1) });
                }
                else
                {
                    Id = Guid.Parse(CId);
                }
                return Id;
            }
        }
        public IActionResult Index()
        {
           CartModel cart = _cartService.GetCartDetails(CartId);
            return View(cart);
        }

        [Route("Cart/AddToCart/{ItemId}/{UnitPrice}/{Quantity}")]
        public IActionResult AddToCart(int ItemId, decimal UnitPrice, int Quantity)
        {
            int UserId = CurrentUser != null ? CurrentUser.Id : 0;  
            if(ItemId > 0 && Quantity > 0)
            {
                Cart cart = _cartService.AddItem(UserId, CartId, ItemId, UnitPrice, Quantity);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var data = JsonSerializer.Serialize(cart, options);
                return Json(data);
            }
            return Json(null);
        }


        [Route("Cart/UpdateQuantity/{Id}/{Quantity}")]
        public IActionResult UpdateQuantity(int Id, int Quantity)
        {
            int count = _cartService.UpdateQuantity(CartId, Id, Quantity);
            return Json(count);
        }

        public IActionResult DeleteItem(int Id)
        {
            int count = _cartService.DeleteItem(CartId, Id);
            return Json(count);
        }
        public IActionResult GetCartCount()
        {
            if (CartId != null)
            {
                int count = _cartService.GetCartCount(CartId);
                return Json(count);
            }
            return Json(0);
        }

        public IActionResult CheckOut()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CheckOut(AddressModel model)
        {
            if (ModelState.IsValid)
            {
                CartModel cart = _cartService.GetCartDetails(CartId);
                if (cart != null && CurrentUser != null)
                {
                    _cartService.UpdateCart(cart.Id, CurrentUser.Id);
                }
                TempData.Set("Cart", cart);
                TempData.Set("Address", model);
                return RedirectToAction("Index", "Payment");
            }
            return View();
        }
    }
}

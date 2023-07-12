using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Implementations
{
    public class CartService : Service<Cart>, ICartService
    {
        ICartRepository _cartRepo;
        IRepository<CartItem> _cartItem;
        IConfiguration _configuration;

        public CartService(ICartRepository cartRepo, IRepository<CartItem> cartItem,IConfiguration configuration) : base(cartRepo)
        {
            _cartRepo = cartRepo;   
            _cartItem = cartItem;
            _configuration = configuration;
        }

        public Cart AddItem(int UserId, Guid CartId, int ItemId, decimal UnitPrice, int Quantity)
        {
            Cart cart = _cartRepo.GetCart(CartId);
            if (cart == null)
            {
                cart = new Cart();
                CartItem item = new CartItem { ItemId = ItemId, Quantity = Quantity, UnitPrice = UnitPrice, CartId = CartId };

                cart.Id = CartId;
                cart.UserId = UserId;
                cart.CreatedDate = DateTime.Now;
                cart.IsActive = true;

                cart.CartItems.Add(item);
                _cartRepo.Add(cart);
                _cartRepo.SaveChanges();
            }
            else
            {
                CartItem cartItem = cart.CartItems.Where(c => c.ItemId == ItemId).FirstOrDefault();
                if (cartItem != null)
                {
                    cartItem.Quantity += Quantity;
                    _cartItem.Update(cartItem);
                    _cartItem.SaveChanges();
                }
                else
                {
                    CartItem item = new CartItem { ItemId = ItemId, Quantity = Quantity, UnitPrice = UnitPrice, CartId = CartId };

                    cart.CartItems.Add(item);

                    _cartItem.Update(item);
                    _cartItem.SaveChanges();
                }
            }
            return cart;
        }

        public int DeleteItem(Guid cartId, int ItemId)
        {
            return _cartRepo.DeleteItem(cartId, ItemId);
        }

        public int GetCartCount(Guid cartId)
        {
            var cart = _cartRepo.GetCart(cartId);
            return cart != null ? cart.CartItems.Count() : 0;
        }

        public CartModel GetCartDetails(Guid cartId)
        {
            var model = _cartRepo.GetCartDetails(cartId);
            if(model != null && model.Items.Count > 0)
            {
                decimal subTotal = 0;
                foreach(var item in model.Items)
                {
                    item.Total = item.UnitPrice * item.Quantity;
                    subTotal += item.Total;
                }
                model.Total = subTotal;
                model.Tax = Math.Round((model.Total * Convert.ToInt32(_configuration["Tax:GST"])) / 100, 2);
                model.GrandTotal = model.Tax + model.Total;
            }
            return model;
        }

        public int UpdateCart(Guid CartId, int UserId)
        {
            return _cartRepo.UpdateCart(CartId, UserId);
        }

        public int UpdateQuantity(Guid CartId, int Id, int Quantity)
        {
            return _cartRepo.UpdateQuantity(CartId, Id, Quantity);
        }
    }
}

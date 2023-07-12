using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Repositories.Implementations
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext db) : base(db)
        {
        }

        public int DeleteItem(Guid cartId, int itemId)
        {
            var item = _db.CartItems.Where(ci => ci.CartId == cartId && ci.ItemId == itemId).FirstOrDefault();
            if (item != null)
            {
                _db.CartItems.Remove(item);
                return _db.SaveChanges();
            }
            return 0;
        }

        public Cart GetCart(Guid CartId)
        {
            return _db.Carts.Include(c => c.CartItems).Where(c => c.Id == CartId && c.IsActive == true).FirstOrDefault();
        }

        public CartModel GetCartDetails(Guid CartId)
        {
            var model = (from c in _db.Carts
                         where c.Id == CartId && c.IsActive == true
                         select new CartModel
                         {
                             Id = c.Id,
                             UserId = c.UserId,
                             CreatedDate = c.CreatedDate,
                             Items = (from cartItem in _db.CartItems
                                      join item in _db.Items
                                      on cartItem.ItemId equals item.Id
                                      where cartItem.CartId == CartId
                                      select new ItemModel
                                      {
                                          Id = cartItem.Id,
                                          Quantity = cartItem.Quantity,
                                          UnitPrice = cartItem.UnitPrice,
                                          ItemId = item.Id,
                                          Name = item.Name,
                                          Description = item.Description,
                                          ImageUrl = item.ImageUrl
                                      }).ToList()
                         }).FirstOrDefault();
            return model;
        }

        public int UpdateCart(Guid cartId, int userId)
        {
            Cart cart = GetCart(cartId);
            cart.UserId = userId;
            return _db.SaveChanges();
        }

        public int UpdateQuantity(Guid cartId, int itemId, int Quantity)
        {
            bool flag = false;
            var cart = GetCart(cartId);
            if(cart != null)
            {
                var cartItems = cart.CartItems.ToList();
                for(int i=0; i<cartItems.Count; i++)
                {
                    if (cartItems[i].Id == itemId)
                    {
                        cartItems[i].Quantity += Quantity;
                        flag = true;
                        break;
                    }
                }
                if(flag) 
                { 
                    cart.CartItems= cartItems;
                    return _db.SaveChanges();
                }
            }
            return 0;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ShopOnlineDbContext shopOnlineDbContext;

        public ShoppingCartRepository(ShopOnlineDbContext shopOnlineDbContext)
        {
            this.shopOnlineDbContext = shopOnlineDbContext;
        }

        private async Task<bool> CartItemExists(int cartId, int productId) =>
            await shopOnlineDbContext.CartItems.AnyAsync(c => c.CartId == cartId && c.ProductId == productId);

        public async Task<CartItem> AddItem(CartItemToAddDto cartItemToAddDto)
        {
            if (await CartItemExists(cartItemToAddDto.CartId, cartItemToAddDto.ProductId) == false)
            {
                var item = await shopOnlineDbContext.Products.Where(p => p.Id == cartItemToAddDto.ProductId).Select(p =>
                    new CartItem
                    {
                        CartId = cartItemToAddDto.CartId,
                        ProductId = p.Id,
                        Qty = cartItemToAddDto.Qty
                    }
                ).SingleOrDefaultAsync();
                if (item != null)
                {
                    var result = await shopOnlineDbContext.CartItems.AddAsync(item);
                    await shopOnlineDbContext.SaveChangesAsync();
                    return result.Entity;
                }
            }
            return null;
        }

        public Task<CartItem> DeleteItem(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CartItem> GetItem(int id)
        {
            return await shopOnlineDbContext.Carts
                .Join(shopOnlineDbContext.CartItems
                .Where(ci => ci.Id == id),
                    cart => cart.Id,
                    cartItem => cartItem.CartId,
                    (cart, cartItem) => new CartItem
                    {
                        Id = cartItem.Id,
                        ProductId = cartItem.ProductId,
                        Qty = cartItem.Qty,
                        CartId = cartItem.CartId
                    }).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<CartItem>> GetItems(int userId)
        {
            return await shopOnlineDbContext.Carts
                .Where(c => c.UserId == userId)
                .Join(shopOnlineDbContext.CartItems,
                    cart => cart.Id,
                    cartItem => cartItem.CartId,
                    (cart, cartItem) => new CartItem
                    {
                        Id = cartItem.Id,
                        ProductId = cartItem.ProductId,
                        Qty = cartItem.Qty,
                        CartId = cartItem.CartId
                    }).ToListAsync();
        }

        public Task<CartItem> UpdateQty(int id, CartItemToAddDto cartItemToAddDto)
        {
            throw new NotImplementedException();
        }
    }
}
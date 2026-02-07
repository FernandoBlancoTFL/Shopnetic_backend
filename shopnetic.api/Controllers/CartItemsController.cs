using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shopnetic.api.Data;
using shopnetic.api.Dto;
using shopnetic.api.Models;

namespace shopnetic.api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CartItemsController : ControllerBase
    {
        public readonly AppDbContext _context;

        public CartItemsController(AppDbContext context)
        {
            _context = context;
        }

        private CartDto ToDto(Cart cart) => new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Total = cart.Total,
            TotalDiscountedProducts = Math.Round(cart.TotalDiscountedProducts, 2),
            TotalProducts = cart.TotalProducts,
            TotalQuantity = cart.TotalQuantity,
            Items = cart.Items?
                .Select(c => new CartItemDto
                {
                    Id = c.Id,
                    CartId = c.CartId,
                    ProductId = c.ProductId,
                    ProductTitle = c.Product.Title,
                    ProductImage = c.Product.Images.OrderBy(i => i.Id).FirstOrDefault()?.Url,
                    Quantity = c.Quantity,
                    Stock = c.Stock,
                    Total = c.Total,
                    DiscountedTotal = Math.Round(c.DiscountedTotal, 2)
                }).ToList() ?? new List<CartItemDto>()
        };

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                return userId;
            return null;
        }

        [HttpPost]
        public async Task<ActionResult<CartItemRequestDto>> CreateCartItem(CartItemRequestDto request)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.Images)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = (int)userId,
                        Items = new List<CartItem>()
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Verificar si el producto existe
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == request.ProductId);

                if (product == null)
                    return NotFound("Product not found");

                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == request.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity = request.Quantity;
                    existingItem.Total = (decimal)(existingItem.Quantity * product.Price);
                    existingItem.DiscountedTotal = (decimal)(existingItem.Total - (existingItem.Total * product.DiscountPercentage / 100));
                    existingItem.Stock = product.Stock - existingItem.Quantity;
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        Total = (decimal)(request.Quantity * product.Price),
                        DiscountedTotal = (decimal)((request.Quantity * product.Price) - ((request.Quantity * product.Price) * product.DiscountPercentage / 100)),
                        Stock = product.Stock - request.Quantity
                    };
                    _context.CartItems.Add(newItem);
                }

                cart.TotalQuantity = cart.Items.Sum(i => i.Quantity);
                cart.Total = cart.Items.Sum(i => i.Total);
                cart.TotalProducts = cart.Items.Count;
                cart.TotalDiscountedProducts = cart.Items.Sum(i => i.DiscountedTotal);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(ToDto(cart));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al agregar producto al carrito: {ex.Message}");
            }
        }


        [HttpDelete("{productid}")]
        public async Task<ActionResult> DeleteCartItemByProductId(int productid)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
                return NotFound();

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productid);
            if (cartItem == null)
                return NotFound("Cart item not found");

            _context.CartItems.Remove(cartItem);

            cart.Total -= cartItem.Total;
            cart.TotalDiscountedProducts -= cartItem.DiscountedTotal;
            cart.TotalProducts -= 1;
            cart.TotalQuantity -= cartItem.Quantity;

            await _context.SaveChangesAsync();

            return Ok(ToDto(cart));
        }

        [HttpDelete("empty-car")]
        public async Task<ActionResult> EmptyCarByUserId()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
                return NotFound();

            _context.CartItems.RemoveRange(cart.Items);
            cart.Total = 0.00M;
            cart.TotalDiscountedProducts = 0.00M;
            cart.TotalProducts = 0;
            cart.TotalQuantity = 0;

            await _context.SaveChangesAsync();

            return Ok(ToDto(cart));
        }

        [HttpPut("{productId}")]
        public async Task<ActionResult<CartDto>> UpdateCartItem(int productId, CartItemRequestDto cartItemRequestDto)
        {
            if (productId != cartItemRequestDto.ProductId)
                return BadRequest("Mismatched product ID");

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
                return NotFound("Cart not found");

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);
            if (cartItem == null)
                return NotFound("Cart item not found");

            if (cartItemRequestDto.Quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
                cart.Items.Remove(cartItem);
            }
            else
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return NotFound("Product not found");

                cartItem.Quantity = cartItemRequestDto.Quantity;
                cartItem.Total = (decimal)(cartItem.Quantity * product.Price);
                cartItem.DiscountedTotal = (decimal)(cartItem.Total - (cartItem.Total * product.DiscountPercentage / 100));
            }

            cart.TotalQuantity = cart.Items.Sum(i => i.Quantity);
            cart.Total = cart.Items.Sum(i => i.Total);
            cart.TotalProducts = cart.Items.Count;
            cart.TotalDiscountedProducts = cart.Items.Sum(i => i.DiscountedTotal);

            await _context.SaveChangesAsync();

            return Ok(ToDto(cart));
        }


    }
}
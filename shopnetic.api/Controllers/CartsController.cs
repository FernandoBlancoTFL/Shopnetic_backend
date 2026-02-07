using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CartsController : ControllerBase
    {
        public readonly AppDbContext _context;

        public CartsController(AppDbContext context)
        {
            _context = context;
        }

        private CartDto ToDto(Cart cart) => new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Total = cart.Total,
            TotalDiscountedProducts = cart.TotalDiscountedProducts,
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
                    DiscountedTotal = c.DiscountedTotal
                }).ToList() ?? new List<CartItemDto>()
        };

        [HttpGet("{id}")]
        public async Task<ActionResult<CartDto>> GetCartByUserId(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == id);
            if (cart == null)
                return NotFound();

            return ToDto(cart);
        }
    }
}
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
    public class OrderItemsController : ControllerBase
    {
        public readonly AppDbContext _context;

        public OrderItemsController(AppDbContext context)
        {
            _context = context;
        }

        private OrderDto ToDto(Order order) => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Subtotal = order.Subtotal,
            Tax = order.Tax,
            ShipmentPrice = order.ShipmentPrice,
            Total = order.Total,
            Status = order.Status,
            Items = order.Items?
                .Select(o => new OrderItemDto
                {
                    ProductId = o.ProductId,
                    ProductTitle = o.Product.Title,
                    ProductImage = o.Product.Images.OrderBy(i => i.Id).FirstOrDefault()?.Url,
                    Price = o.Price,
                    Quantity = o.Quantity,
                    Category = o.Product.Category.Name,
                    Brand = o.Product.Brand,
                    Weight = o.Product.Weight,
                    Width = o.Product.Dimensions.Width,
                }).ToList() ?? new List<OrderItemDto>()
        };

        [HttpPost("{userId}")]
        public async Task<ActionResult<OrderDto>> CreateOrderItemByUserCart(int userId)
        {

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return NotFound("Cart not found");

            if (!cart.Items.Any())
            {
                return BadRequest("No products found in cart");
            }

            var tax = 10.00M;
            var shipmentPrice = 0.00M;

            var order = new Order
            {
                UserId = (int)userId,
                Subtotal = cart.TotalDiscountedProducts,
                Tax = tax,
                ShipmentPrice = shipmentPrice,
                Total = cart.TotalDiscountedProducts + tax + shipmentPrice,
                Status = "pending",
                Items = new List<OrderItem>()
            };

            if (cart.Items.Any())
            {
                foreach (CartItem cartItem in cart.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
                    if (product == null) return NotFound("Product not found");

                    var newItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.DiscountedTotal,
                    };
                    order.Items.Add(newItem);
                }
            }

            _context.Orders.Add(order);

            if (cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                cart.Total = 0;
                cart.TotalDiscountedProducts = 0;
                cart.TotalProducts = 0;
                cart.TotalQuantity = 0;
            }

            await _context.SaveChangesAsync();

            var savedOrder = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Dimensions)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return ToDto(savedOrder);
        }
    }
}
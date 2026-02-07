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
    public class OrdersController : ControllerBase
    {
        public readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
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
            Total = order.Subtotal + order.Tax + order.ShipmentPrice,
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
                    Width = o.Product.Weight
                }).ToList() ?? new List<OrderItemDto>()
        };

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                return userId;
            return null;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserId()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var orders = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Where(o => o.UserId == userId).ToListAsync();
            if (orders == null)
                return NotFound();

            return orders.Select(ToDto).ToList();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, OrderRequestDto orderRequestDto)
        {
            if (id != orderRequestDto.OrderId)
                return BadRequest();

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == orderRequestDto.OrderId);
            if (order == null)
                return NotFound();

            if (orderRequestDto.Status != null)
            {
                order.Status = orderRequestDto.Status;

                foreach (var item in order.Items)
                {
                    var product = item.Product;
                    if (product != null)
                    {
                        if (product.Stock - item.Quantity < 0)
                        {
                            return BadRequest("Not enough product stock!");
                        }
                        product.Stock -= item.Quantity;
                    }
                }
            }

            if (orderRequestDto.ShipmentPrice != null)
            {
                order.ShipmentPrice = (decimal)orderRequestDto.ShipmentPrice;
            }

            await _context.SaveChangesAsync();

            return ToDto(order);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shopnetic.api.Data;
using shopnetic.api.Dto;
using shopnetic.api.Models;

namespace shopnetic.api.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;
        public OrdersService(AppDbContext context)
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

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Where(o => o.UserId == userId).ToListAsync();

            return orders.Select(ToDto).ToList();
        }
        
        public async Task<OrderDto> UpdateOrderAsync(int id, OrderRequestDto orderRequestDto)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == id);
            
            if (order == null) 
                return null;
            
            // Validación de stock
            if (orderRequestDto.Status != null)
            {
                foreach (var item in order.Items)
                {
                    if (item.Product != null && item.Product.Stock - item.Quantity < 0)
                    {
                        throw new InvalidOperationException("Not enough product stock!");
                    }
                }
                
                order.Status = orderRequestDto.Status;
                foreach (var item in order.Items)
                {
                    if (item.Product != null)
                        item.Product.Stock -= item.Quantity; //Ver si realmente está descontando la cantidad
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
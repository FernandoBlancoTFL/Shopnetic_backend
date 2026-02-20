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
using shopnetic.api.Services;

namespace shopnetic.api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

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
            
            var orders = await _ordersService.GetOrdersByUserIdAsync(userId.Value);

            if (!orders.Any())
                return NotFound();
            
            return Ok(orders);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, OrderRequestDto orderRequestDto)
        {
            if (id != orderRequestDto.OrderId)
                return BadRequest();
            
            try
            {
                var orderDto = await _ordersService.UpdateOrderAsync(id, orderRequestDto);
                
                if (orderDto == null)
                    return NotFound();
                
                return orderDto;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
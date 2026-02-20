using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shopnetic.API.Dto;

namespace Shopnetic.API.Services
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto> UpdateOrderAsync(int id, OrderRequestDto orderRequestDto);
    }
}
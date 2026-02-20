using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopnetic.api.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShipmentPrice { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
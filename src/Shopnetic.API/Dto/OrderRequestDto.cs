using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopnetic.API.Dto
{
    public class OrderRequestDto
    {
        public int OrderId { get; set; }
        public string? Status { get; set; }
        public decimal? ShipmentPrice { get; set; }
    }
}
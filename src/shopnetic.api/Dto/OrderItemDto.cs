using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shopnetic.API.Models;

namespace Shopnetic.API.Dto
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public decimal Weight { get; set; }
        public double Width { get; set; }
    }
}
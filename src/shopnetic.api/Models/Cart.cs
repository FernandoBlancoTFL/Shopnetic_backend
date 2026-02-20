using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopnetic.API.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal Total { get; set; }
        public decimal TotalDiscountedProducts { get; set; }
        public int TotalProducts { get; set; }
        public int TotalQuantity { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
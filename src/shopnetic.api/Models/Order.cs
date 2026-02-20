using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Shopnetic.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShipmentPrice { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
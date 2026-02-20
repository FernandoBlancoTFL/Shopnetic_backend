using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using shopnetic.api.Models;

namespace shopnetic.api.Dto
{
    public class CartDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public decimal Total { get; set; }
        [Required]
        public decimal TotalDiscountedProducts { get; set; }
        [Required]
        public int TotalProducts { get; set; }
        [Required]
        public int TotalQuantity { get; set; }
        [Required]
        public ICollection<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace shopnetic.api.Dto
{
    public class CartItemDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int CartId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ProductTitle { get; set; }
        [Required]
        public string ProductImage { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int Stock { get; set; }

        [Required]
        public decimal Total { get; set; }
        [Required]
        public decimal DiscountedTotal { get; set; }
    }
}
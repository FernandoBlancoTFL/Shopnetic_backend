using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Shopnetic.API.Models;

namespace Shopnetic.API.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        [Required]
        public String Title { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        public String Category { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal DiscountPercentage { get; set; }
        [Required]
        public double Rating { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public String Brand { get; set; }
        [Required]
        public String Sku { get; set; }
        [Required]
        public int Weight { get; set; }
        [Required]
        public Dimensions Dimensions { get; set; }
        [Required]
        public String WarrantyInformation { get; set; }
        [Required]
        public String ShippingInformation { get; set; }
        [Required]
        public String AvailabilityStatus { get; set; }
        [Required]
        public ICollection<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        [Required]
        public String ReturnPolicy { get; set; }
        [Required]
        public int MinimumOrderQuantity { get; set; }
        [Required]
        public Meta Meta { get; set; }
        [Required]
        public List<string> Images { get; set; } = new List<string>();
        [Required]
        public String Thumbnail { get; set; }
    }
}

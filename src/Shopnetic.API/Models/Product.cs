using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopnetic.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public String Brand { get; set; }
        public String Sku { get; set; }
        public int Weight { get; set; }
        public Dimensions Dimensions { get; set; }
        public String WarrantyInformation { get; set; }
        public String ShippingInformation { get; set; }
        public String AvailabilityStatus { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public String ReturnPolicy { get; set; }
        public int MinimumOrderQuantity { get; set; }
        public Meta Meta { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public String Thumbnail { get; set; }
    }
}

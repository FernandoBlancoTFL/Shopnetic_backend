using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopnetic.API.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public String Url { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopnetic.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public String Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

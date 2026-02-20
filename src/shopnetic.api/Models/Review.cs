using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopnetic.api.Models
{
    public class Review
    {
        public int Id { get; set;}
        public int Rating { get; set; }
        public String Comment { get; set; }
        public DateTime Date { get; set; }
        public String ReviewerName { get; set; }
        public String ReviewerEmail { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

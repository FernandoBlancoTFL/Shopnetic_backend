using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopnetic.api.Dto
{
    public class ReviewDto
    {
        public int Rating { get; set; }
        public String Comment { get; set; }
        public DateTime Date { get; set; }
        public String ReviewerName { get; set; }
        public String ReviewerEmail { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace shopnetic.api.Models
{
    [Owned]
    public class Meta
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public String Barcode { get; set; }
        public String qrCode { get; set; }
    }
}
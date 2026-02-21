using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopnetic.API.Dto
{
    public class CategoryDto
    {
        [Required]
        public String Name { get; set; }
    }
}
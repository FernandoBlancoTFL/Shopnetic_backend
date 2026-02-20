using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace shopnetic.api.Models
{
    public class UserDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public DateTime Created_at { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Image { get; set; }
    }
}

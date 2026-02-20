using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopnetic.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public DateTime Created_at { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}

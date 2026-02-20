using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shopnetic.api.Data;
using shopnetic.api.Models;

namespace shopnetic.api.Controllers
{
    [ApiController]
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        private UserDto ToDto(User user) => new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            Password = user.PasswordHash,
            Description = user.Description,
            Country = user.Country,
            Created_at = user.Created_at,
            Role = user.Role,
            Image = user.Image
        };

        private User ToEntity(UserDto userDto)
        {
            var user = new User
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                UserName = userDto.UserName,
                Email = userDto.Email,
                Description = userDto.Description,
                Country = userDto.Country,
                Created_at = userDto.Created_at,
                Role = userDto.Role,
                Image = userDto.Image
            };

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, userDto.Password);

            return user;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(ToDto).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return ToDto(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(UserDto userDto)
        {
            var user = ToEntity(userDto);
            _context.Add(user);
            await _context.SaveChangesAsync();

            userDto.Id = user.Id;
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UserDto userDto)
        {
            if (id != userDto.Id)
                return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, userDto.Password);
            user.Description = userDto.Description;
            user.Country = userDto.Country;
            user.Role = userDto.Role;
            user.Image = userDto.Image;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("reset-db")]
        public IActionResult ResetDatabase()
        {
            _context.CartItems.RemoveRange(_context.CartItems);
            _context.Carts.RemoveRange(_context.Carts);
            _context.Categories.RemoveRange(_context.Categories);
            _context.OrderItems.RemoveRange(_context.OrderItems);
            _context.Orders.RemoveRange(_context.Orders);
            _context.Products.RemoveRange(_context.Products);
            _context.ProductsImages.RemoveRange(_context.ProductsImages);
            _context.Reviews.RemoveRange(_context.Reviews);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('CartItems', RESEED, 0);");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Carts', RESEED, 0);");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Categories', RESEED, 0);");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('OrderItems', RESEED, 0);");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Orders', RESEED, 0);");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Products', RESEED, 0);");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('ProductsImages', RESEED, 0);");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Reviews', RESEED, 0);");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Users', RESEED, 0);");

            SeedData.Initialize(_context);

            return Ok(new { message = "Database restarted with initial data" });
        }
    }
}
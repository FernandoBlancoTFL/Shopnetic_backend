using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopnetic.API.Data;
using Shopnetic.API.Dto;
using Shopnetic.API.Models;

namespace Shopnetic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        private ProductDto ToDto(Product product) => new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Category = product.Category.Name,
            Price = product.Price,
            DiscountPercentage = product.DiscountPercentage,
            Rating = product.Rating,
            Stock = product.Stock,
            Brand = product.Brand,
            Sku = product.Sku,
            Weight = product.Weight,
            Dimensions = product.Dimensions,
            WarrantyInformation = product.WarrantyInformation,
            ShippingInformation = product.ShippingInformation,
            AvailabilityStatus = product.AvailabilityStatus,
            Reviews = product.Reviews?
                .Select(r => new ReviewDto
                {
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Date = r.Date,
                    ReviewerName = r.ReviewerName,
                    ReviewerEmail = r.ReviewerEmail
                }).ToList() ?? new List<ReviewDto>(),
            ReturnPolicy = product.ReturnPolicy,
            MinimumOrderQuantity = product.MinimumOrderQuantity,
            Meta = product.Meta,
            Images = product.Images?.Select(i => i.Url).ToList() ?? new List<string>(),
            Thumbnail = product.Thumbnail
        };

        [HttpGet]
        public async Task<ActionResult<object>> GetProducts(
            [FromQuery] int limit = 30,
            [FromQuery] int skip = 0,
            [FromQuery] string select = "",
            [FromQuery] string sortBy = "",
            [FromQuery] string order = "asc")
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.Images)
                .Include(p => p.Category);

            int total = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var sortProperty = sortBy.ToLower();
                var sortOrder = order.ToLower();

                query = (sortProperty, sortOrder) switch
                {
                    ("title", "asc") => query.OrderBy(p => p.Title),
                    ("title", "desc") => query.OrderByDescending(p => p.Title),
                    ("price", "asc") => query.OrderBy(p => p.Price),
                    ("price", "desc") => query.OrderByDescending(p => p.Price),
                    ("category", "asc") => query.OrderBy(p => p.Category),
                    ("category", "desc") => query.OrderByDescending(p => p.Category),
                    ("brand", "asc") => query.OrderBy(p => p.Brand),
                    ("brand", "desc") => query.OrderByDescending(p => p.Brand),
                    ("stock", "asc") => query.OrderBy(p => p.Stock),
                    ("stock", "desc") => query.OrderByDescending(p => p.Stock),
                    _ => query
                };
            }

            query = query.Skip(skip).Take(limit);

            var products = await query.ToListAsync();

            IEnumerable<object> result;

            if (string.IsNullOrWhiteSpace(select))
            {
                result = products.Select(ToDto).ToList<object>();
            }
            else
            {
                var fields = select.Split(',').Select(f => f.Trim().ToLower()).ToHashSet();

                result = products.Select(p =>
                {
                    var dict = new Dictionary<string, object>();
                    if (fields.Contains("id")) dict["id"] = p.Id;
                    if (fields.Contains("title")) dict["title"] = p.Title;
                    if (fields.Contains("price")) dict["price"] = p.Price;
                    if (fields.Contains("category")) dict["category"] = p.Category;
                    if (fields.Contains("brand")) dict["brand"] = p.Brand;
                    if (fields.Contains("stock")) dict["stock"] = p.Stock;
                    return dict;
                });
            }

            return Ok(new
            {
                total,
                limit,
                skip,
                products = result
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return ToDto(product);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<object>> GetProductsByCategory(string category)
        {
            var products = await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Where(p => p.Category.Name.Contains(category))
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound();
            }

            var result = products.Select(ToDto).ToList();

            return Ok(new
            {
                total = result.Count,
                products = result
            });
        }


        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchProducts([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest("Debe proporcionar un término de búsqueda con el parámetro 'q'.");
            }

            var products = await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Where(p =>
                    p.Title.Contains(q) ||
                    p.Description.Contains(q) ||
                    p.Category.Name.Contains(q) ||
                    p.Brand.Contains(q) ||
                    p.Sku.Contains(q))
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound("No se encontraron productos que coincidan con la búsqueda.");
            }

            var result = products.Select(ToDto).ToList();

            return Ok(new
            {
                total = result.Count,
                products = result
            });
        }

        [HttpGet("category-list")]
        public async Task<ActionResult<IEnumerable<String>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => c.Name)
                .ToListAsync();
            return Ok(categories);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Shopnetic.API.Data;
using Shopnetic.API.Models;

namespace Shopnetic.API
{
    public static class SeedData
    {
        public static void Initialize(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Seed(context);
        }

        public static void Initialize(AppDbContext context)
        {
            Seed(context);
        }

        private static void Seed(AppDbContext context)
        {
            if (!context.Products.Any())
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "products.json");

                if (!File.Exists(filePath))
                    return;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var jsonData = File.ReadAllText(filePath);
                var products = JsonSerializer.Deserialize<List<Product>>(jsonData, options);

                if (products == null || !products.Any())
                    return;

                var uniqueCategories = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);

                foreach (var product in products)
                {
                    if (string.IsNullOrWhiteSpace(product.Brand))
                    {
                        product.Brand = "N/A";
                    }

                    if (product.Category == null || string.IsNullOrWhiteSpace(product.Category.Name))
                        continue;

                    var categoryName = product.Category.Name.Trim();

                    if (!uniqueCategories.ContainsKey(categoryName))
                    {
                        var newCategory = new Category { Name = categoryName };
                        uniqueCategories[categoryName] = newCategory;
                        context.Categories.Add(newCategory);
                    }

                    product.Category = uniqueCategories[categoryName];
                }

                context.Products.AddRange(products);
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                var filePathUsers = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.json");

                if (!File.Exists(filePathUsers))
                    return;

                var optionsUsers = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var jsonDataUsers = File.ReadAllText(filePathUsers);
                var users = JsonSerializer.Deserialize<List<User>>(jsonDataUsers, optionsUsers);

                if (users == null || !users.Any())
                    return;

                var passwordHasher = new PasswordHasher<User>();

                foreach (var user in users)
                {
                    var plainPassword = user.PasswordHash;
                    user.PasswordHash = passwordHasher.HashPassword(user, plainPassword);
                    user.Created_at = DateTime.UtcNow;
                }

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
    }
}
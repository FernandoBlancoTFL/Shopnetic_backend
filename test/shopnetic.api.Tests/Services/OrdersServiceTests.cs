using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using shopnetic.api.Data;
using shopnetic.api.Models;
using shopnetic.api.Services;
using Microsoft.EntityFrameworkCore;

namespace shopnetic.api.Tests.Services
{
    public class OrdersServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetOrdersByUserIdAsync_ReturnsOnlyOrdersOfThatUser()
        {
            using var context = GetDbContext();

            context.Orders.AddRange(
                new Order
                {
                    UserId = 1,
                    Status = "pending",
                    Subtotal = 10,
                    Tax = 20,
                    ShipmentPrice = 30,
                    Total = 60
                },
                new Order
                {
                    UserId = 2,
                    Status = "completed",
                    Subtotal = 50,
                    Tax = 10,
                    ShipmentPrice = 5,
                    Total = 65
                }
            );

            await context.SaveChangesAsync();

            var service = new OrdersService(context);

            var orders = await service.GetOrdersByUserIdAsync(1);

            Assert.Single(orders);
            Assert.All(orders, o => Assert.Equal("pending", o.Status));
        }

        // [Fact]
        // public async Task UpdateOrderAsync_ModifyOrderInDatabase()
        // {
        //     using var context = GetDbContext();
        //     var service = new OrdersService(context);
        //     var id = 1;
        //     var order = new OrderRequestDto { OrderId = 1, Status = "completed", ShipmentPrice = 30 };

        //     var result = await service.UpdateOrderAsync(id, orderRequestDto);

        //     Assert.NotNull(result);
        //     Assert.Equal(1, result.UserId);
        //     Assert.Equal("completed", result.Status);
        //     Assert.True(result.Id > 0);
        // }

        
    }
}
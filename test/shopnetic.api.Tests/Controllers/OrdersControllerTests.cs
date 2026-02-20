using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Shopnetic.API.Services;
using Shopnetic.API.Controllers;
using Shopnetic.API.Dto;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Shopnetic.API.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrdersService> _ordersServiceMock;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _ordersServiceMock = new Mock<IOrdersService>();
            _controller = new OrdersController(_ordersServiceMock.Object);
        }

        [Fact]
        public async Task GetOrders_ReturnsOrders_WhenExists()
        {
            var userId = 3;

            var orders = new List<OrderDto>
            {
                new OrderDto
                {
                    Id = 1,
                    UserId = userId,
                    Status = "pending",
                    Subtotal = 10,
                    Tax = 20,
                    ShipmentPrice = 30,
                    Total = 60
                }
            };

            _ordersServiceMock
                .Setup(s => s.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(orders);

            // Simulación usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            var result = await _controller.GetOrdersByUserId();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<OrderDto>>(okResult.Value);

            Assert.Single(returnValue);
            Assert.Equal(userId, returnValue.First().UserId);
        }

        [Fact]
        public async Task GetOrdersByUserId_ReturnsUnauthorized_WhenUserIsNull()
        {
            // Arrange
            // No seteamos HttpContext.User, que simulaba un usuario autenticado

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.GetOrdersByUserId();

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetOrdersByUserId_ReturnsNotFound_WhenUserHasNoOrders()
        {
            var userId = 5;

            _ordersServiceMock
                .Setup(s => s.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(new List<OrderDto>());

            // Simulación usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            var result = await _controller.GetOrdersByUserId();

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
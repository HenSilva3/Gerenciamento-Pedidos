using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entitys;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockProductRepo = new Mock<IProductRepository>();
            _mockOrderRepo = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrderService>>();
            _orderService = new OrderService(_mockProductRepo.Object, _mockOrderRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldFail_WhenStockIsInsufficient()
        {
            // ARRANGE
            var productInStock = new Product { Id = 1, Name = "Caneta", StockQuantity = 5 };
            var itemsToOrder = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 10 } };

            _mockProductRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(productInStock);

            // ACT
            var result = await _orderService.CreateOrderAsync(1, itemsToOrder);

            // ASSERT
            Assert.False(result.Success); 
            Assert.Equal("Estoque insuficiente para o produto Caneta.", result.ErrorMessage); 
            _mockOrderRepo.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Never); 
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldSucceed_WhenStockIsSufficient()
        {
            // ARRANGE
            var productInStock = new Product { Id = 1, Name = "Caneta", StockQuantity = 20 };
            var itemsToOrder = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 10, UnitPrice = 2 } };

            _mockProductRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(productInStock);
            _mockOrderRepo.Setup(repo => repo.AddAsync(It.IsAny<Order>())).ReturnsAsync(123); 

            // ACT
            var result = await _orderService.CreateOrderAsync(1, itemsToOrder);

            // ASSERT
            Assert.True(result.Success); 
            Assert.Null(result.ErrorMessage); 
            Assert.Equal(123, result.NewOrderId); 
            _mockProductRepo.Verify(repo => repo.UpdateStockAsync(1, 10), Times.Once); 
        }
    }
}

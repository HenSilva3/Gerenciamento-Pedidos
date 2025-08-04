
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entitys;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IProductRepository productRepository, IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<(bool Success, string ErrorMessage, int NewOrderId)> CreateOrderAsync(int clientId, List<OrderItem> items)
        {
            _logger.LogInformation("Iniciando avalidações!");
            foreach (var item in items)
            {
                var productInDb = await _productRepository.GetByIdAsync(item.ProductId);
                if (productInDb == null)
                {
                    return (false, $"Produto com ID {item.ProductId} não encontrado.", 0);
                }
                if (productInDb.StockQuantity < item.Quantity)
                {
                    return (false, $"Estoque insuficiente para o produto {productInDb.Name}.", 0);
                }
            }

            var order = new Order
            {
                ClientId = clientId,
                OrderDate = System.DateTime.Now,
                Status = "Novo",
                Items = items,
                TotalPrice = items.Sum(i => i.Quantity * i.UnitPrice)
            };

            _logger.LogInformation("Persistindo a ordem!");
            var newOrderId = await _orderRepository.AddAsync(order);

            _logger.LogInformation("Atualizando o estoque!");
            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                var newQuantity = product.StockQuantity - item.Quantity;
                await _productRepository.UpdateStockAsync(item.ProductId, newQuantity);
            }

            _logger.LogInformation("Processamento concluido com sucesso!");
            return (true, null, newOrderId);
        }
    }
}

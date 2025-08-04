using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Web.Models;

namespace OrderManagement.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;

        public OrdersController(
            IOrderRepository orderRepository,
            IClientRepository clientRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
        }

        // GET: /Orders
        // Lista todos os pedidos com filtros
        public async Task<IActionResult> Index(int? clientId, string status)
        {
            var orders = await _orderRepository.GetAllAsync();
            var clients = await _clientRepository.GetAllAsync();

            ViewBag.Clients = new SelectList(clients, "Id", "Name", clientId);
            ViewBag.Statuses = new SelectList(new List<string> { "Novo", "Processando", "Finalizado" }, status);

            if (clientId.HasValue)
            {
                orders = orders.Where(o => o.ClientId == clientId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status == status);
            }

            var viewModel = orders.Select(o => new OrderListViewModel
            {
                Id = o.Id,
                ClientName = clients.FirstOrDefault(c => c.Id == o.ClientId)?.Name ?? "Cliente não encontrado",
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Status = o.Status
            }).ToList();

            return View(viewModel);
        }
    }
}

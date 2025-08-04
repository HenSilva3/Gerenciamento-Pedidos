using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entitys;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Web.Models;

namespace OrderManagement.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderService _orderService;

        public OrdersController(
            IOrderRepository orderRepository,
            IClientRepository clientRepository,
            IProductRepository productRepository,
            IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
            _orderService = orderService;
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

        // GET: Orders/Create
        // Prepara e exibe o formulário de criação de pedido.
        public async Task<IActionResult> Create()
        {
            var clients = await _clientRepository.GetAllAsync();
            ViewBag.Clients = new SelectList(clients, "Id", "Name");
            return View();
        }

        // POST: Orders/Create
        // Recebe os dados do formulário e salva o novo pedido.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateOrderViewModel model)
        {
            if (model == null || model.ClientId == 0 || model.Items == null || !model.Items.Any())
            {
                return BadRequest("Cliente ou itens do pedido inválidos.");
            }

            var result = await _orderService.CreateOrderAsync(model.ClientId, model.Items);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = result.NewOrderId }) });
        }

        // GET: Orders/SearchProducts
        // Action auxiliar para busca de produtos.
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<Product>());
            }

            var allProducts = await _productRepository.GetAllAsync();
            var filteredProducts = allProducts
                .Where(p => p.Name.ToLower().Contains(term.ToLower()) && p.StockQuantity > 0)
                .Select(p => new { id = p.Id, name = p.Name, price = p.Price, stock = p.StockQuantity })
                .ToList();

            return Json(filteredProducts);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetByIdAsync(order.ClientId);

            var viewModel = new OrderDetailViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                CurrentStatus = order.Status,
                TotalPrice = order.TotalPrice,
                ClientName = client?.Name ?? "N/A",
                ClientEmail = client?.Email ?? "N/A",
                Items = new List<OrderItemViewModel>()
            };

            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                viewModel.Items.Add(new OrderItemViewModel
                {
                    ProductName = product?.Name ?? "Produto não encontrado",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            ViewBag.Statuses = new SelectList(new List<string> { "Novo", "Processando", "Finalizado" }, viewModel.CurrentStatus);

            return View(viewModel);
        }

        // POST: Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
        {
            if (string.IsNullOrEmpty(newStatus))
            {
                TempData["ErrorMessage"] = "Por favor, selecione um novo status para atualizar.";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            await _orderRepository.UpdateStatusAsync(orderId, newStatus);

            TempData["SuccessMessage"] = $"O status do pedido #{orderId} foi atualizado para '{newStatus}' com sucesso!";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}

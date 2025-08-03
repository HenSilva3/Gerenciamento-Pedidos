using Microsoft.AspNetCore.Mvc;
using OrderManagement.Domain.Entitys;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Web.Controllers
{
    
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // GET: /Clients
        // Lista todos os clientes com filtro por nome ou e-mail
        public async Task<IActionResult> Index(string searchString)
        {
            var clients = await _clientRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                var lowerCaseSearch = searchString.ToLower();
                clients = clients.Where(c =>
                    c.Name.ToLower().Contains(lowerCaseSearch) ||
                    c.Email.ToLower().Contains(lowerCaseSearch)
                ).ToList();
            }

            return View(clients);
        }

        // GET: Clients/Create
        // Mostra o formulário para criar um novo cliente
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // Recebe os dados do formulário e cria o cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Phone")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.CreationDate = DateTime.Now;
                await _clientRepository.AddAsync(client);
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        // Mostra o formulário para editar um cliente existente
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // Recebe os dados do formulário e atualiza o cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,CreationDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _clientRepository.UpdateAsync(client);
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }


        // GET: Clients/Delete/5
        // Mostra a página de confirmação de exclusão
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        // Executa a exclusão
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clientRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

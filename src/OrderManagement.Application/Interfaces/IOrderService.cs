using OrderManagement.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Interfaces
{
    public interface IOrderService
    {
        Task<(bool Success, string ErrorMessage, int NewOrderId)> CreateOrderAsync(int clientId, List<OrderItem> items);
    }
}

using OrderManagement.Domain.Entitys;

namespace OrderManagement.Web.Models
{
    public class CreateOrderViewModel
    {
        public int ClientId { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}

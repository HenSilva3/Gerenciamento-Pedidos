namespace OrderManagement.Web.Models
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string CurrentStatus { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }
}

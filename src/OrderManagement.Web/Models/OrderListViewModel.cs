namespace OrderManagement.Web.Models
{
    public class OrderListViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}

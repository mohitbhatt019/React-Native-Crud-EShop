namespace Shopping_Api.Models.ModelDto_s
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductName { get; set; } 
        public decimal ProductPrice { get; set; } 
    }
}

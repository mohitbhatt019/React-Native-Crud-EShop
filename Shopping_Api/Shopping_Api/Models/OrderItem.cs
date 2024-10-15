using System.ComponentModel.DataAnnotations;

namespace Shopping_Api.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Shopping_Api.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [Required]
        [StringLength(100, ErrorMessage = "Name is too long.")]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Shipping Address is too long.")]
        public string ShippingAddress { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Billing Address is too long.")]
        public string BillingAddress { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

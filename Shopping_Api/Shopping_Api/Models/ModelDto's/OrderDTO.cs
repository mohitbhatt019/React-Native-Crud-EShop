using System.ComponentModel.DataAnnotations;

namespace Shopping_Api.Models.ModelDto_s
{
    public class OrderDTO
    {
        public int Id { get; set; }

        [Required]
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();

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

        [Required]
        [StringLength(100, ErrorMessage = "Name is too long.")]
        public string CustomerName { get; set; }

        // Add TotalPrice field
        public decimal TotalPrice { get; set; }
    }
}

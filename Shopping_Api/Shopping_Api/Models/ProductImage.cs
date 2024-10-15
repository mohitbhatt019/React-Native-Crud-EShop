using System.ComponentModel.DataAnnotations;

namespace Shopping_Api.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        public string ImagePath { get; set; }

        public bool IsDefault { get; set; }

        public Product Product { get; set; }
    }
}

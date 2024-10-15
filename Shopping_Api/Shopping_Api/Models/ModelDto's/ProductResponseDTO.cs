namespace Shopping_Api.Models.ModelDto_s
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public ProductType Type { get; set; }
        public List<ProductImageResponseDTO> Images { get; set; }
    }
}

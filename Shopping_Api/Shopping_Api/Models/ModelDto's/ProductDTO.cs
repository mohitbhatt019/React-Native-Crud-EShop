﻿using System.ComponentModel.DataAnnotations;

namespace Shopping_Api.Models.ModelDto_s
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Height { get; set; }


        public decimal Width { get; set; }


        public decimal Length { get; set; }

        [Required]
        public ProductType Type { get; set; }

        public List<IFormFile> Images { get; set; }
    }

    public enum ProductType
    {
        Electronics,
        Clothing,
        Food,
        Furniture
    }
}

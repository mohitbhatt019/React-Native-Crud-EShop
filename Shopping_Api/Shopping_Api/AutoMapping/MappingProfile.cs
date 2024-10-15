using AutoMapper;
using Shopping_Api.Models.ModelDto_s;
using Shopping_Api.Models;

namespace Shopping_Api.AutoMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add this missing mapping configuration
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());  // Add specific mappings as needed

            // Mapping configuration from ProductDTO to Product.
            // The Images property is ignored during this mapping
            CreateMap<ProductDTO, Product>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            // Mapping configuration from Product to ProductResponseDTO.
            // The Images property is mapped from the source model
            CreateMap<Product, ProductResponseDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

            // Mapping configuration from ProductImage to ProductImageResponseDTO.
            CreateMap<ProductImage, ProductImageResponseDTO>();

            // Bi-directional mapping configuration between OrderDTO and Order.
            // Order Mappings
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderDTO, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            // OrderItem Mappings
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price));

            CreateMap<OrderItemDTO, OrderItem>();
        }
    }
}

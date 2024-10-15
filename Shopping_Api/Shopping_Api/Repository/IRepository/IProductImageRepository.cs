using Shopping_Api.Models;

namespace Shopping_Api.Repository.IRepository
{
    public interface IProductImageRepository
    {
        Task AddAsync(ProductImage productImage);
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId);
        Task DeleteAsync(int id);
    }
}

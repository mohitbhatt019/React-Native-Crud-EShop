using Microsoft.EntityFrameworkCore;
using Shopping_Api.ApplicationDb;
using Shopping_Api.Models;
using Shopping_Api.Repository.IRepository;

namespace Shopping_Api.Repository
{
    /// <summary>
    /// Implementation of IProductImageRepository for managing product image-related operations.
    /// </summary>
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ProductImageRepository, initializing the ApplicationDbContext.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new product image to the database.
        /// </summary>
        /// <param name="productImage">The product image to be added.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAsync(ProductImage productImage)
        {
            await _context.ProductImages.AddAsync(productImage); // Add the product image to the context
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        /// <summary>
        /// Retrieves all images associated with a specific product by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>A list of product images associated with the specified product.</returns>
        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId) =>
            await _context.ProductImages.Where(pi => pi.ProductId == productId).ToListAsync(); // Get all images related to the product by product ID

        /// <summary>
        /// Deletes a product image from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the product image to be deleted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteAsync(int id)
        {
            var image = await _context.ProductImages.FindAsync(id); // Find the image by ID
            if (image != null)
            {
                _context.ProductImages.Remove(image); // Remove the image from the context
                await _context.SaveChangesAsync(); // Save changes to the database
            }
        }
    }
}

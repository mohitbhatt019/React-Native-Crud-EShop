using Microsoft.EntityFrameworkCore;
using Shopping_Api.ApplicationDb;
using Shopping_Api.Models;
using Shopping_Api.Repository.IRepository;

namespace Shopping_Api.Repository
{
    /// <summary>
    /// Implementation of IProductRepository for managing product-related operations.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ProductRepository, initializing the ApplicationDbContext.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a product by its ID, including associated images.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product matching the specified ID, or null if not found.</returns>
        public async Task<Product> GetByIdAsync(int id) => await _context.Products
            .Include(p => p.Images) // Include associated images for the product
            .FirstOrDefaultAsync(p => p.Id == id); // Retrieve the product with the given ID

        /// <summary>
        /// Retrieves all products from the database, including associated images.
        /// </summary>
        /// <returns>A list of all products.</returns>
        public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products
            .Include(p => p.Images) // Include associated images for each product
            .ToListAsync(); // Convert the result to a list

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product to be added.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product); // Add the product to the context
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="product">The product to be updated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product); // Update the product in the context
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        /// <summary>
        /// Deletes a product from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to be deleted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id); // Retrieve the product by ID
            if (product != null)
            {
                _context.Products.Remove(product); // Remove the product from the context
                await _context.SaveChangesAsync(); // Save changes to the database
            }
        }
    }
}

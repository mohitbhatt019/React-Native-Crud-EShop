using Microsoft.EntityFrameworkCore;
using Shopping_Api.ApplicationDb;
using Shopping_Api.Models;
using Shopping_Api.Repository.IRepository;

namespace Shopping_Api.Repository
{
    /// <summary>
    /// Implementation of IOrderRepository for managing order-related operations.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for OrderRepository, initializing the ApplicationDbContext.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves an order by its ID, including associated order items and product details.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <returns>The order matching the specified ID, or null if not found.</returns>
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems) // Include associated order items
                .ThenInclude(oi => oi.Product) // Include associated product details for each order item
                .FirstOrDefaultAsync(o => o.Id == id); // Retrieve the order with the given ID
        }

        /// <summary>
        /// Retrieves all orders from the database, including associated order items and product details.
        /// </summary>
        /// <returns>A list of all orders.</returns>
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems) // Include associated order items
                .ThenInclude(oi => oi.Product) // Include associated product details for each order item
                .ToListAsync(); // Convert the result to a list
        }

        /// <summary>
        /// Adds a new order to the database.
        /// </summary>
        /// <param name="order">The order to be created.</param>
        public async Task CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order); // Add the order to the context
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        /// <summary>
        /// Updates an existing order in the database.
        /// </summary>
        /// <param name="order">The order to be updated.</param>
        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order); // Update the order in the context
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        /// <summary>
        /// Deletes an order from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to be deleted.</param>
        public async Task DeleteOrderAsync(int id)
        {
            var order = await GetOrderByIdAsync(id); // Retrieve the order by ID
            if (order != null)
            {
                _context.Orders.Remove(order); // Remove the order from the context
                await _context.SaveChangesAsync(); // Save changes to the database
            }
        }
    }
}

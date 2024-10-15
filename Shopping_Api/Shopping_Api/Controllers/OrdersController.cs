using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shopping_Api.Models;
using Shopping_Api.Models.ModelDto_s;
using Shopping_Api.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping_Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper, ILogger<OrdersController> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>A list of all orders with HTTP status 200 (OK), or HTTP status 404 (Not Found) if no orders are found.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            _logger.LogInformation("Fetching all orders from the database.");

            var orders = await _orderRepository.GetAllOrdersAsync();

            if (orders != null && orders.Any())
            {
                _logger.LogInformation($"{orders.Count()} orders found.");

                var products = await _productRepository.GetAllAsync();
                var productDictionary = products.ToDictionary(p => p.Id);

                var orderResponseDTOs = orders.Select(order =>
                {
                    // Map order to DTO
                    var orderDto = _mapper.Map<OrderDTO>(order);

                    // Enrich order items with product details
                    orderDto.OrderItems = order.OrderItems.Select(oi =>
                    {
                        var orderItemDto = _mapper.Map<OrderItemDTO>(oi);
                        if (productDictionary.TryGetValue(oi.ProductId, out var product))
                        {
                            // Enrich the order item with product details
                            orderItemDto.ProductName = product.Name; // Assuming you add ProductName to OrderItemDTO
                            orderItemDto.ProductPrice = product.Price; // Assuming you add ProductPrice to OrderItemDTO
                        }
                        return orderItemDto;
                    }).ToList();

                    return orderDto;
                }).ToList();

                return Ok(orderResponseDTOs);
            }
            _logger.LogWarning("No orders found.");

            return NotFound("No orders found.");
        }


        /// <summary>
        /// Retrieves an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order with HTTP status 200 (OK), or HTTP status 404 (Not Found) if the order is not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            _logger.LogInformation($"Fetching order with ID {id}"); 

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found."); 
                return NotFound();
            }

            var orderResponseDTO = _mapper.Map<OrderDTO>(order);
            return Ok(orderResponseDTO);
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="orderCreateDTO">The DTO containing the details of the order to create.</param>
        /// <returns>HTTP status 201 (Created) with the created order details, or HTTP status 400 (Bad Request) for validation errors.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderCreateDTO)
        {
            _logger.LogInformation("Creating a new order.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for order creation.");

                return BadRequest(ModelState);
            }

            var products = await _productRepository.GetAllAsync();
            var productDict = products.ToDictionary(p => p.Id);

            foreach (var item in orderCreateDTO.OrderItems)
            {
                if (!productDict.ContainsKey(item.ProductId))
                {
                    _logger.LogWarning($"Product ID {item.ProductId} is invalid.");

                    return BadRequest("Invalid product ID.");
                }

                var product = productDict[item.ProductId];
                if (product.Quantity < item.Quantity)
                {
                    _logger.LogWarning($"Insufficient quantity for product ID {item.ProductId}.");

                    return BadRequest("Insufficient product quantity.");
                }
            }

            var order = _mapper.Map<Order>(orderCreateDTO);
            order.TotalPrice = orderCreateDTO.OrderItems.Sum(item => item.ProductPrice * item.Quantity);

            foreach (var item in order.OrderItems)
            {
                var product = productDict[item.ProductId];
                product.Quantity -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            await _orderRepository.CreateOrderAsync(order);
            var orderResponseDTO = _mapper.Map<OrderDTO>(order);

            _logger.LogInformation($"Order created successfully with ID {order.Id}.");

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, orderResponseDTO);
        }


        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="orderDto">The DTO containing the updated details of the order.</param>
        /// <returns>HTTP status 204 (No Content) if the update is successful, or HTTP status 400 (Bad Request) if the IDs do not match or validation fails.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO orderDto)
        {
            _logger.LogInformation($"Updating order with ID {id}.");

            if (id != orderDto.Id)
            {
                _logger.LogWarning("Order ID mismatch.");

                return BadRequest();
            }

            var existingOrder = await _orderRepository.GetOrderByIdAsync(id);
            if (existingOrder == null)
            {
                _logger.LogWarning($"Order with ID {id} not found.");

                return NotFound();
            }

            var order = _mapper.Map<Order>(orderDto);

            // Update the product quantities
            foreach (var item in existingOrder.OrderItems)
            {
                var prod = await _productRepository.GetByIdAsync(item.ProductId);
                prod.Quantity += item.Quantity; // Return the old quantity to stock
                await _productRepository.UpdateAsync(prod);
            }

            foreach (var item in orderDto.OrderItems)
            {
                var prod = await _productRepository.GetByIdAsync(item.ProductId);
                prod.Quantity -= item.Quantity; // Deduct the new quantity from stock
                await _productRepository.UpdateAsync(prod);
            }

            await _orderRepository.UpdateOrderAsync(order);

            _logger.LogInformation($"Order with ID {id} updated successfully.");

            return NoContent();
        }

        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>HTTP status 200 (OK) if the order is deleted successfully, or HTTP status 404 (Not Found) if the order is not found.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation($"Deleting order with ID {id}."); 

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found.");

                return NotFound();
            }

            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity += item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            await _orderRepository.DeleteOrderAsync(id);
            _logger.LogInformation($"Order with ID {id} deleted successfully.");

            return NoContent();
        }

        /// <summary>
        /// Retrieves a summary of orders grouped by product.
        /// </summary>
        /// <returns>A summary of total orders for each product with HTTP status 200 (OK), or HTTP status 404 (Not Found) if no orders are found.</returns>
        [HttpGet("product-summary")]
        public async Task<IActionResult> GetOrdersByProduct()
        {
            _logger.LogInformation("Generating product summary from orders.");

            var orders = await _orderRepository.GetAllOrdersAsync();
            if (orders == null || !orders.Any())
            {
                _logger.LogWarning("No orders found for product summary.");

                return NotFound("No orders found.");
            }

            var ordersByProduct = orders
                .SelectMany(order => order.OrderItems)
                .GroupBy(oi => oi.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalOrders = group.Sum(g => g.Quantity)
                })
                .ToList();

            var products = await _productRepository.GetAllAsync();
            var productDict = products.ToDictionary(p => p.Id, p => p.Name);

            var result = ordersByProduct
                .Select(item => new
                {
                    ProductName = productDict.ContainsKey(item.ProductId) ? productDict[item.ProductId] : "Unknown",
                    TotalOrders = item.TotalOrders
                })
                .ToList();
            _logger.LogInformation("Product summary generated successfully.");

            return Ok(result);
        }
    }
}

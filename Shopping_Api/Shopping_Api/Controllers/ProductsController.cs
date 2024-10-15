using Microsoft.AspNetCore.Mvc;
using Shopping_Api.Models;
using Shopping_Api.Models.ModelDto_s;
using Shopping_Api.Repository.IRepository;
using AutoMapper;

namespace Shopping_Api.Controllers
{
    // Define the route for the controller and specify that it is an API controller
    [Route("api/product")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Declare private fields for repository interfaces, AutoMapper, image folder path, and logger
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IMapper _mapper;
        private readonly string _imageFolderPath;
        private readonly ILogger<ProductsController> _logger;

        // Constructor for dependency injection of repositories, AutoMapper, and logger
        public ProductsController(
            IProductRepository productRepository,
            IProductImageRepository productImageRepository,
            IMapper mapper,
            ILogger<ProductsController> logger)
        {
            _productRepository = productRepository; // Inject product repository
            _productImageRepository = productImageRepository; // Inject product image repository
            _mapper = mapper; // Inject AutoMapper
            _logger = logger; // Inject logger
            _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images"); // Set image folder path

            // Ensure the image folder exists; create it if it doesn't
            if (!Directory.Exists(_imageFolderPath))
            {
                Directory.CreateDirectory(_imageFolderPath);
                _logger.LogInformation("Image folder created at path: {ImageFolderPath}", _imageFolderPath);
            }
        }

        // GET: api/product
        // Retrieves all products from the database
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation("Attempting to retrieve all products.");

            // Fetch all products from the repository
            var products = await _productRepository.GetAllAsync();

            // Check if products are null or empty
            if (products == null || !products.Any())
            {
                _logger.LogWarning("No products found.");
                return NotFound("No products found."); // Return 404 if no products found
            }

            // Map products to a list of ProductResponseDTO
            var productResponseDTOs = _mapper.Map<List<ProductResponseDTO>>(products);

            // Log the retrieved products
            _logger.LogInformation("Retrieved all products: {@ProductResponseDTOs}", productResponseDTOs);

            // Return the list of products
            return Ok(productResponseDTOs);
        }

        // POST: api/product
        // Creates a new product with optional images
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDTO productDTO, [FromForm] List<IFormFile> images)
        {
            _logger.LogInformation("Attempting to create a new product with details: {@ProductDTO}", productDTO);

            // Validate model state
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for product creation.");
                return BadRequest(ModelState); // Return 400 if model state is invalid
            }

            // Map DTO to product entity
            var product = _mapper.Map<Product>(productDTO);

            // Add the new product to the repository
            await _productRepository.AddAsync(product);
            _logger.LogInformation("Product created with ID: {ProductId}", product.Id);

            // Check if images are provided
            if (images != null && images.Count > 0)
            {
                var imageList = new List<ProductImage>(); // Initialize list for product images
                for (int i = 0; i < images.Count; i++)
                {
                    var image = images[i];
                    if (image.Length > 0)
                    {
                        var fileName = Path.GetFileName(image.FileName); // Get image file name
                        var filePath = Path.Combine(_imageFolderPath, fileName); // Construct file path

                        // Save image to server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                            _logger.LogInformation("Image saved at path: {FilePath}", filePath);
                        }

                        // Create ProductImage entity
                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImagePath = $"/images/{fileName}",
                            IsDefault = (i == 0) // Set the first image as the default
                        };

                        imageList.Add(productImage); // Add to image list
                    }
                }

                // Save all product images to the repository
                foreach (var productImage in imageList)
                {
                    await _productImageRepository.AddAsync(productImage);
                    _logger.LogInformation("Image record saved with ProductId: {ProductId} and ImagePath: {ImagePath}", productImage.ProductId, productImage.ImagePath);
                }
            }

            // Map the product entity to ProductResponseDTO and return it
            var productResponseDTO = _mapper.Map<ProductResponseDTO>(await _productRepository.GetByIdAsync(product.Id));
            _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, productResponseDTO);
        }

        // GET: api/product/{id}
        // Retrieves a product by its ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            _logger.LogInformation("Attempting to retrieve product with ID: {ProductId}", id);

            // Fetch product by ID from the repository
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                return NotFound(); // Return 404 if product not found
            }

            // Map product entity to ProductResponseDTO
            var productDTO = _mapper.Map<ProductResponseDTO>(product);
            _logger.LogInformation("Product retrieved successfully with ID: {ProductId}", id);

            return Ok(productDTO); // Return the product details
        }

        // PUT: api/product
        // Updates an existing product with optional new images
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductDTO productDTO, [FromForm] List<IFormFile>? images)
        {
            _logger.LogInformation("Attempting to update product with ID: {ProductId}", productDTO.Id);

            // Fetch the existing product by ID
            var existingProduct = await _productRepository.GetByIdAsync(productDTO.Id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", productDTO.Id);
                return NotFound(); // Return 404 if product not found
            }

            // Map updated details from DTO to the existing product entity
            _mapper.Map(productDTO, existingProduct);
            await _productRepository.UpdateAsync(existingProduct); // Update the product
            _logger.LogInformation("Product updated with ID: {ProductId}", productDTO.Id);

            // Check if new images are provided
            if (images != null && images.Count > 0)
            {
                _logger.LogInformation("New images provided for product with ID: {ProductId}", productDTO.Id);

                // Fetch old images for the product
                var oldImages = await _productImageRepository.GetByProductIdAsync(productDTO.Id);
                foreach (var image in oldImages)
                {
                    var oldFilePath = Path.Combine(_imageFolderPath, Path.GetFileName(image.ImagePath)); // Get old image file path
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath); // Delete old image file
                        _logger.LogInformation("Deleted old image file at path: {OldFilePath}", oldFilePath);
                    }
                    await _productImageRepository.DeleteAsync(image.Id); // Delete old image record
                    _logger.LogInformation("Deleted old image record with ID: {ImageId}", image.Id);
                }

                // Save new images
                var imageList = new List<ProductImage>(); // Initialize list for new images
                for (int i = 0; i < images.Count; i++)
                {
                    var image = images[i];
                    if (image.Length > 0)
                    {
                        var fileName = Path.GetFileName(image.FileName); // Get image file name
                        var filePath = Path.Combine(_imageFolderPath, fileName); // Construct file path

                        // Save image to server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                            _logger.LogInformation("New image saved at path: {FilePath}", filePath);
                        }

                        // Create ProductImage entity
                        var productImage = new ProductImage
                        {
                            ProductId = existingProduct.Id,
                            ImagePath = $"/images/{fileName}",
                            IsDefault = (i == 0) // Set the first image as the default
                        };

                        imageList.Add(productImage); // Add to image list
                    }
                }

                // Save all new images to the repository
                foreach (var productImage in imageList)
                {
                    await _productImageRepository.AddAsync(productImage);
                    _logger.LogInformation("New image record saved with ProductId: {ProductId} and ImagePath: {ImagePath}", productImage.ProductId, productImage.ImagePath);
                }
            }

            // Map the updated product entity to ProductResponseDTO and return it
            var updatedProductDTO = _mapper.Map<ProductResponseDTO>(existingProduct);
            _logger.LogInformation("Product updated successfully with ID: {ProductId}", productDTO.Id);

            return Ok(updatedProductDTO);
        }

        // DELETE: api/product/{id}
        // Deletes a product and its associated images by product ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Attempting to delete product with ID: {ProductId}", id);

            // Fetch the product by ID
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                return NotFound(); // Return 404 if product not found
            }

            // Fetch associated images for the product
            var images = await _productImageRepository.GetByProductIdAsync(id);
            foreach (var image in images)
            {
                var filePath = Path.Combine(_imageFolderPath, Path.GetFileName(image.ImagePath)); // Get image file path
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath); // Delete image file from server
                    _logger.LogInformation("Deleted image file at path: {FilePath}", filePath);
                }
                await _productImageRepository.DeleteAsync(image.Id); // Delete image record from database
                _logger.LogInformation("Deleted image record with ID: {ImageId}", image.Id);
            }

            // Delete the product from the repository
            await _productRepository.DeleteAsync(id);
            _logger.LogInformation("Product deleted with ID: {ProductId}", id);

            return NoContent(); // Return 204 No Content after deletion
        }
    }
}

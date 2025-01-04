using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Data;
using NewEra_Cash___Carry.DTOs.product;
using NewEra_Cash___Carry.Models;
using Serilog;

namespace NewEra_Cash___Carry.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;

        public ProductController(RetailOrderingSystemDbContext context)
        {
            _context = context;
        }

        // Get all products - Accessible to any user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Select(p => new ProductDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Stock = p.Stock,
                        CategoryName = p.Category.Name,
                        ImageUrls = p.ProductImages.Select(pi => pi.ImageUrl).ToList()
                    })
                    .ToListAsync();

                Log.Information("Fetched all products successfully.");
                return Ok(products);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching products.");
                throw;
            }
        }

        // Search products with filters
        [HttpGet("search")]
        public async Task<ActionResult> SearchProducts(
            [FromQuery] string? name,
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Page and pageSize must be greater than 0." });
            }

            try
            {
                var query = _context.Products.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(name)) query = query.Where(p => p.Name.Contains(name));
                if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId);
                if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
                if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);

                // Apply sorting
                query = sortBy?.ToLower() switch
                {
                    "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                    _ => query
                };

                // Paginate and fetch results
                var totalItems = await query.CountAsync();
                var products = await query
                    .Include(p => p.Category)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                    Data = products
                };

                Log.Information("Search products completed successfully.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during product search.");
                throw;
            }
        }

        // Get product by ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                {
                    Log.Warning("Product with ID {ProductId} not found.", id);
                    return NotFound(new { message = "Product not found." });
                }

                Log.Information("Product with ID {ProductId} retrieved successfully.", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving product with ID {ProductId}.", id);
                throw;
            }
        }

        // Create a new product - Only accessible to Admins
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> PostProduct([FromBody] ProductPostDto productDto)
        {
            // Check if the body is empty or null
            if (productDto == null)
            {
                Log.Warning("Empty body provided for product creation.");
                return BadRequest(new { message = "Request body cannot be null or empty." });
            }

            // Validate product details
            if (string.IsNullOrWhiteSpace(productDto.Name))
            {
                Log.Warning("Product creation failed due to missing name.");
                return BadRequest(new { message = "Product name is required." });
            }

            if (productDto.Price <= 0)
            {
                Log.Warning("Product creation failed due to invalid price.");
                return BadRequest(new { message = "Product price must be greater than zero." });
            }

            if (!await _context.Categories.AnyAsync(c => c.CategoryId == productDto.CategoryId))
            {
                Log.Warning("Product creation failed due to invalid CategoryId: {CategoryId}", productDto.CategoryId);
                return BadRequest(new { message = "Invalid CategoryId. The category does not exist." });
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId
            };

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                Log.Information("Product {ProductName} created with ID {ProductId}.", product.Name, product.ProductId);
                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating product {ProductName}.", productDto.Name);
                throw;
            }
        }


        // Update product
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductPostDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Product not found." });

            if (!await _context.Categories.AnyAsync(c => c.CategoryId == productDto.CategoryId))
            {
                return BadRequest(new { message = "Invalid CategoryId. The category does not exist." });
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Stock = productDto.Stock;
            product.CategoryId = productDto.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                Log.Information("Product with ID {ProductId} updated successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating product with ID {ProductId}.", id);
                throw;
            }
        }

        // Delete product
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Product not found." });

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                Log.Information("Product with ID {ProductId} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting product with ID {ProductId}.", id);
                throw;
            }
        }

        // Image upload for product
        [HttpPost("{id}/upload-images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImages(int id, List<IFormFile> files)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Product not found." });

            if (files == null || files.Count == 0)
            {
                return BadRequest(new { message = "No files uploaded." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var imagePath = Path.Combine("wwwroot", "images");

            if (!Directory.Exists(imagePath)) Directory.CreateDirectory(imagePath);

            var productImages = new List<ProductImage>();

            foreach (var file in files)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = $"Invalid file type: {file.FileName}" });
                }

                if (file.Length > 5 * 1024 * 1024) // 5 MB
                {
                    return BadRequest(new { message = $"File too large: {file.FileName}" });
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                productImages.Add(new ProductImage
                {
                    ProductId = id,
                    ImageUrl = $"/images/{fileName}"
                });
            }

            try
            {
                _context.ProductImages.AddRange(productImages);
                await _context.SaveChangesAsync();
                Log.Information("Images uploaded successfully for product ID {ProductId}.", id);
                return Ok(productImages.Select(pi => new { pi.Id, pi.ImageUrl }));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error uploading images for product ID {ProductId}.", id);
                throw;
            }
        }
    }
}

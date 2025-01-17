using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewEra_Cash___Carry.Application.Interfaces.ProductInterfaces;
using NewEra_Cash___Carry.Core.DTOs.product;
using Serilog;

namespace NewEra_Cash___Carry.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                Log.Information("Fetched all products successfully.");
                return Ok(products);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Error fetching products.");
                throw;
            }
        }

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
            try
            {
                var products = await _productService.SearchProductsAsync(name, categoryId, minPrice, maxPrice, page, pageSize, sortBy, ascending);
                Log.Information("Search products completed successfully.");
                return Ok(products);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Error during product search.");
                throw;
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                Log.Information("Product with ID {ProductId} retrieved successfully.", id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                Log.Warning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Error retrieving product with ID {ProductId}.", id);
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> PostProduct([FromBody] ProductPostDto productDto)
        {
            try
            {
                var product = await _productService.CreateProductAsync(productDto);
                Log.Information("Product created successfully.");
                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
            }
            catch (KeyNotFoundException ex)
            {
                Log.Warning(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Error creating product.");
                throw;
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductPostDto productDto)
        {
            try
            {
                await _productService.UpdateProductAsync(id, productDto);
                Log.Information("Product with ID {ProductId} updated successfully.", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Log.Warning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Error updating product with ID {ProductId}.", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                Log.Information("Product with ID {ProductId} deleted successfully.", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                Log.Warning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Error deleting product with ID {ProductId}.", id);
                throw;
            }
        }

        [HttpPost("{id}/upload-images")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadImages(int id, List<IFormFile> files)
        {
            try
            {
                var uploadedImages = await _productService.UploadImagesAsync(id, files);
                Log.Information("Images uploaded successfully for product ID {ProductId}.", id);
                return Ok(uploadedImages);
            }
            catch (KeyNotFoundException ex)
            {
                Log.Warning(ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Error uploading images for product ID {ProductId}.", id);
                throw;
            }
        }
    }
}

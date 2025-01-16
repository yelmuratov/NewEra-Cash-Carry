using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Core.DTOs.product;
using NewEra_Cash___Carry.Infrastructure.Data;
using Serilog;

namespace NewEra_Cash___Carry.API.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsV2Controller : ControllerBase // Inherit from ControllerBase
    {
        private readonly RetailOrderingSystemDbContext _context;

        public ProductsV2Controller(RetailOrderingSystemDbContext context)
        {
            _context = context;
        }

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
                return Ok(products); // Ok method now works
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching products.");
                throw;
            }
        }
    }
}

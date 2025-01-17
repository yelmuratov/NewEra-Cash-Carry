using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.DTOs.product;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.Infrastructure.Data;

namespace NewEra_Cash___Carry.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly RetailOrderingSystemDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(RetailOrderingSystemDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) throw new KeyNotFoundException("Product not found.");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string name, int? categoryId, decimal? minPrice, decimal? maxPrice, int page, int pageSize, string sortBy, bool ascending)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(name)) query = query.Where(p => p.Name.Contains(name));
            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId);
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice);

            query = sortBy?.ToLower() switch
            {
                "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                _ => query
            };

            var products = await query
                .Include(p => p.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(ProductPostDto productDto)
        {
            if (!await _context.Categories.AnyAsync(c => c.CategoryId == productDto.CategoryId))
            {
                throw new KeyNotFoundException("Invalid CategoryId. The category does not exist.");
            }

            var product = _mapper.Map<Product>(productDto);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductAsync(int id, ProductPostDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            if (!await _context.Categories.AnyAsync(c => c.CategoryId == productDto.CategoryId))
            {
                throw new KeyNotFoundException("Invalid CategoryId. The category does not exist.");
            }

            _mapper.Map(productDto, product);
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<object>> UploadImagesAsync(int id, List<IFormFile> files)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var imagePath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(imagePath)) Directory.CreateDirectory(imagePath);

            var productImages = new List<ProductImage>();
            foreach (var file in files)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension)) throw new Exception($"Invalid file type: {file.FileName}");
                if (file.Length > 5 * 1024 * 1024) throw new Exception($"File too large: {file.FileName}");

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

            _context.ProductImages.AddRange(productImages);
            await _context.SaveChangesAsync();

            return productImages.Select(pi => new { pi.Id, pi.ImageUrl });
        }
    }
}

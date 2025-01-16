using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly RetailOrderingSystemDbContext _context;

        public ProductRepository(RetailOrderingSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToListAsync();
        }

        public async Task<Product> GetProductWithDetailsByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string name, int? categoryId, decimal? minPrice, decimal? maxPrice, int page, int pageSize, string sortBy, bool ascending)
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

            return await query
                .Include(p => p.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}

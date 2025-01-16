using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.Infrastructure.Data;

namespace NewEra_Cash___Carry.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly RetailOrderingSystemDbContext _context;

        public CategoryRepository(RetailOrderingSystemDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CategoryExistsAsync(string categoryName)
        {
            return await _context.Categories.AnyAsync(c => c.Name == categoryName);
        }

        public async Task<bool> CategoryExistsByIdAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }
    }
}

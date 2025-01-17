using NewEra_Cash___Carry.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Application.Interfaces.ProductInterfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync();
        Task<Product> GetProductWithDetailsByIdAsync(int id);
        Task<IEnumerable<Product>> SearchProductsAsync(string name, int? categoryId, decimal? minPrice, decimal? maxPrice, int page, int pageSize, string sortBy, bool ascending);
    }
}

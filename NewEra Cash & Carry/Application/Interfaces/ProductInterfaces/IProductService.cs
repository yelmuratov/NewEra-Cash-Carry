using NewEra_Cash___Carry.Core.DTOs.product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Application.Interfaces.ProductInterfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string name, int? categoryId, decimal? minPrice, decimal? maxPrice, int page, int pageSize, string sortBy, bool ascending);
        Task<ProductDto> CreateProductAsync(ProductPostDto productDto);
        Task UpdateProductAsync(int id, ProductPostDto productDto);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<object>> UploadImagesAsync(int id, List<IFormFile> files);
    }
}

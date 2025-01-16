using AutoMapper;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.DTOs.product;
using NewEra_Cash___Carry.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsWithDetailsAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductWithDetailsByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string name, int? categoryId, decimal? minPrice, decimal? maxPrice, int page, int pageSize, string sortBy, bool ascending)
        {
            var products = await _productRepository.SearchProductsAsync(name, categoryId, minPrice, maxPrice, page, pageSize, sortBy, ascending);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(ProductPostDto productDto)
        {
            if (!await _categoryRepository.CategoryExistsByIdAsync(productDto.CategoryId))
            {
                throw new KeyNotFoundException("Category not found.");
            }

            var product = _mapper.Map<Product>(productDto);
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductAsync(int id, ProductPostDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            if (!await _categoryRepository.CategoryExistsByIdAsync(productDto.CategoryId))
            {
                throw new KeyNotFoundException("Category not found.");
            }

            _mapper.Map(productDto, product);
            product.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            _productRepository.Delete(product);
            await _productRepository.SaveChangesAsync();
        }
    }
}

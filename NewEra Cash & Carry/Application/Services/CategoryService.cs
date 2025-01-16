using AutoMapper;
using NewEra_Cash___Carry.Application.Interfaces;
using NewEra_Cash___Carry.Core.DTOs.category;
using NewEra_Cash___Carry.Core.Entities;

namespace NewEra_Cash___Carry.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }
            return category;
        }

        public async Task<Category> CreateCategoryAsync(CategoryPostDto categoryDto)
        {
            if (await _categoryRepository.CategoryExistsAsync(categoryDto.Name))
            {
                throw new System.Exception("A category with this name already exists.");
            }

            var category = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return category;
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}

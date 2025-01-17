using NewEra_Cash___Carry.Core.DTOs.category;
using NewEra_Cash___Carry.Core.Entities;


namespace NewEra_Cash___Carry.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(CategoryPostDto categoryDto);
        Task UpdateCategoryAsync(int id, CategoryUpdateDto categoryDto);
        Task DeleteCategoryAsync(int id);
    }
}

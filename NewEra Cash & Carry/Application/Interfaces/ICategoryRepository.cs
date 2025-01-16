using NewEra_Cash___Carry.Core.Entities;

namespace NewEra_Cash___Carry.Application.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> CategoryExistsAsync(string categoryName);
        Task<bool> CategoryExistsByIdAsync(int categoryId);
    }
}

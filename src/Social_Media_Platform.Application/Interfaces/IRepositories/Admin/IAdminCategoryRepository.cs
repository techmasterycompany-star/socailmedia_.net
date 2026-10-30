using Social_Media_Platform.Application.DTOS.Admin;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IRepositories.Admin
{
    public interface IAdminCategoryRepository
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<Category?> GetByIdAsync(long id);
        Task<Category?> GetByNameAsync(string name);
        Task DeleteAsync(Category category);
        Task UpdateAsync(Category category);
        Task AddAsync(Category category);
    }
}

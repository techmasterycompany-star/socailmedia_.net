using Social_Media_Platform.Application.Common.DTOs.Admin;

namespace Social_Media_Platform.Application.Interfaces.IServices.Admin
{
    public interface IAdminCategoryService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto> GetCategoryAsync(long id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request , long? actorId);
        Task<CategoryDto> UpdateCategoryAsync(long id ,UpdateCategoryRequest request , long? actorId);
        Task<CommandResult> DeleteCategoryAsync(long id, long? actorId);

    }
}

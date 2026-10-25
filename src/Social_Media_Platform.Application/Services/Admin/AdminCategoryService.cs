using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.DTOS.Admin;
using Social_Media_Platform.Application.Interfaces.IRepositories.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;
using Social_Media_Platform.Domain.Entities;
using System.Threading;

namespace Social_Media_Platform.Application.Services.Admin
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly IAdminCategoryRepository repo;
        public AdminCategoryService(IAdminCategoryRepository repo) => this.repo = repo;


        private const int MaxNameLength = 100;
        private const int MaxDescriptionLength = 1000;

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, long? actorId)
        {
            var name = NormalizeName(request.Name);
            ValidateDescription(request.Description);

            await EnsureNameIsAvailableAsync(name, null);

            var category = new Category
            {
                Name = name,
                Description = request.Description?.Trim(),
                IsActive = request.IsActive
            };

            await repo.AddAsync(category);

            return Map(category);
        }

        public async Task<CommandResult> DeleteCategoryAsync(long id, long? actorId)
        {
            var category = await repo.GetByIdAsync(id)
                ?? throw new NotFoundException("The requested category could not be found.");

            await repo.DeleteAsync(category);

            return new CommandResult($"Category '{category.Name}' deleted.", category.Id);
        }

        public Task<List<CategoryDto>> GetCategoriesAsync() =>
            repo.GetCategoriesAsync();
        

        public async Task<CategoryDto> GetCategoryAsync(long id)
        {
            var category = await repo.GetByIdAsync(id)
                ?? throw new NotFoundException("The requested category could not be found.");

            return Map(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(long id, UpdateCategoryRequest request, long? actorId)
        {
            var name = NormalizeName(request.Name);
            ValidateDescription(request.Description);

            var category = await repo.GetByIdAsync(id)
                ?? throw new NotFoundException("The requested category could not be found.");

            await EnsureNameIsAvailableAsync(name, id);

            category.Name = name;
            category.Description = request.Description?.Trim();
            category.IsActive = request.IsActive;

            await repo.UpdateAsync(category);

            return Map(category);
        }






        private async Task EnsureNameIsAvailableAsync(string name,long? currentId)
        {
            var existing = await repo.GetByNameAsync(name);

            if (existing is not null && existing.Id != currentId)
            {
                throw new ConflictException($"A category named '{name}' already exists.");
            }
        }

        private static string NormalizeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BadRequestException("A category name is required.");
            }

            var trimmed = name.Trim();

            if (trimmed.Length > MaxNameLength)
            {
                throw new BadRequestException($"The category name cannot exceed {MaxNameLength} characters.");
            }

            return trimmed;
        }

        private static void ValidateDescription(string? description)
        {
            if (description is { Length: > MaxDescriptionLength })
            {
                throw new BadRequestException(
                    $"The category description cannot exceed {MaxDescriptionLength} characters.");
            }
        }

        private static CategoryDto Map(Category category) => new()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}

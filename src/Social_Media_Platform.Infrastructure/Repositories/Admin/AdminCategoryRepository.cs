using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.DTOS.Admin;
using Social_Media_Platform.Application.Interfaces.IRepositories.Admin;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories.Admin
{
    public class AdminCategoryRepository : IAdminCategoryRepository
    {
        private readonly Social_Media_PlatformDbContext context;
        public AdminCategoryRepository(Social_Media_PlatformDbContext context) => this.context = context;



        public async Task AddAsync(Category category)
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            context.Remove(category);
            await context.SaveChangesAsync();
        }

        public async Task<Category?> GetByIdAsync(long id) => 
            await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        

        public async Task<Category?> GetByNameAsync(string name) =>
            await context.Categories.FirstOrDefaultAsync(c => c.Name == name);
       

        public async Task<List<CategoryDto>> GetCategoriesAsync() =>
            await context.Categories
               .AsNoTracking()
               .Select(category => new CategoryDto
               {
                  Id = category.Id,
                  Name = category.Name,
                  Description = category.Description,
                  IsActive = category.IsActive,
                  CreatedAt = category.CreatedAt,
                  UpdatedAt = category.UpdatedAt
               })
            .ToListAsync();


        public async Task UpdateAsync(Category category)
        {
            context.Update(category);
            await context.SaveChangesAsync();
        }
    }
}

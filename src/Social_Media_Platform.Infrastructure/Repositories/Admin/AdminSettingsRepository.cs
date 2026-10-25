using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.DTOS.Admin;
using Social_Media_Platform.Application.Interfaces.IRepositories.Admin;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories.Admin
{
    public class AdminSettingsRepository : IAdminSettingsRepository
    {
        private readonly Social_Media_PlatformDbContext context;
        public AdminSettingsRepository(Social_Media_PlatformDbContext context) => this.context = context;
        



        public async Task AddAsync(PlatformSetting setting)
        {
            context.Add(setting);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PlatformSetting setting)
        {
            context.Remove(setting);
            await context.SaveChangesAsync();
        }

        public async Task<PlatformSetting?> GetByKeyAsync(string key) =>
            await context.PlatformSettings.FirstOrDefaultAsync(p => p.Key == key);
        

        public async Task<List<PlatformSettingDto>> GetSettingsAsync() => 
            await context.PlatformSettings.AsNoTracking()
            .Select(setting => new PlatformSettingDto
            {
                Id = setting.Id,
                Key = setting.Key,
                Value = setting.Value,
                Description = setting.Description,
                IsPublic = setting.IsPublic,
                CreatedAt = setting.CreatedAt,
                UpdatedAt = setting.UpdatedAt
            }).ToListAsync();
       

        public async Task UpdateAsync(PlatformSetting setting)
        {
            context.Update(setting);
            await context.SaveChangesAsync();
        }
    }
}

using Social_Media_Platform.Application.Common.DTOs.Admin;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IRepositories.Admin
{
    public interface IAdminSettingsRepository
    {
        Task<List<PlatformSettingDto>> GetSettingsAsync();
        Task<PlatformSetting?> GetByKeyAsync(string key);
        Task AddAsync(PlatformSetting setting);
        Task UpdateAsync(PlatformSetting setting);
        Task DeleteAsync(PlatformSetting setting);
    }
}

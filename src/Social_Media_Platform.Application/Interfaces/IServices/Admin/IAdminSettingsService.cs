using Social_Media_Platform.Application.DTOS.Admin;

namespace Social_Media_Platform.Application.Interfaces.IServices.Admin
{
    public interface IAdminSettingsService
    {
        Task<List<PlatformSettingDto>> GetSettingsAsync();
        Task<PlatformSettingDto> GetSettingAsync(string key);
        Task<PlatformSettingDto> CreateSettingAsync(CreateSettingRequest request ,long? actorId);
        Task<PlatformSettingDto> UpdateSettingAsync(string key , UpdateSettingRequest request, long? actorId);
        Task<CommandResult> DeleteSettingAsync(string key, long? actorId);
    }
}

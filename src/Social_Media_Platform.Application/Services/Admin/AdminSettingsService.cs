using Social_Media_Platform.Application.Common.DTOs.Admin;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.Interfaces.IRepositories.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;
using Social_Media_Platform.Domain.Entities;
using System.Threading;

namespace Social_Media_Platform.Application.Services.Admin
{
    public class AdminSettingsService : IAdminSettingsService
    {
        private readonly IAdminSettingsRepository repo;
        public AdminSettingsService(IAdminSettingsRepository repo) => this.repo = repo;

        private const int MaxKeyLength = 100;
        private const int MaxDescriptionLength = 1000;



        public async Task<PlatformSettingDto> CreateSettingAsync(CreateSettingRequest request, long? actorId)
        {
            var key = NormalizeKey(request.Key);
            ValidateValue(request.Value);
            ValidateDescription(request.Description);

            var existing = await repo.GetByKeyAsync(key);
            if (existing is not null)
            {
                throw new ConflictException($"A setting with key '{key}' already exists.");
            }

            var setting = new PlatformSetting
            {
                Key = key,
                Value = request.Value,
                Description = request.Description?.Trim(),
                IsPublic = request.IsPublic
            };

            await repo.AddAsync(setting);
            return Map(setting);
        }

        public async Task<CommandResult> DeleteSettingAsync(string key, long? actorId)
        {
            var setting = await repo.GetByKeyAsync(NormalizeKey(key))
                ?? throw new NotFoundException("The requested setting could not be found.");

            await repo.DeleteAsync(setting);
            return new CommandResult($"Setting '{setting.Key}' deleted.", setting.Id);
        }

        public async Task<PlatformSettingDto> GetSettingAsync(string key)
        {
            var setting = await repo.GetByKeyAsync(NormalizeKey(key))
                 ?? throw new NotFoundException("The requested setting could not be found.");

            return Map(setting);
        }

        public async Task<List<PlatformSettingDto>> GetSettingsAsync() =>
            await repo.GetSettingsAsync();


        public async Task<PlatformSettingDto> UpdateSettingAsync(string key, UpdateSettingRequest request, long? actorId)
        {
            var normalizedKey = NormalizeKey(key);
            ValidateValue(request.Value);
            ValidateDescription(request.Description);

            var setting = await repo.GetByKeyAsync(normalizedKey)
                ?? throw new NotFoundException("The requested setting could not be found.");

            setting.Value = request.Value;
            setting.Description = request.Description?.Trim();
            setting.IsPublic = request.IsPublic;

            await repo.UpdateAsync(setting);
            return Map(setting);
        }





        private static string NormalizeKey(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new BadRequestException("A setting key is required.");
            }

            var trimmed = key.Trim();

            if (trimmed.Length > MaxKeyLength)
            {
                throw new BadRequestException($"The setting key cannot exceed {MaxKeyLength} characters.");
            }

            if (trimmed.Any(char.IsWhiteSpace))
            {
                throw new BadRequestException("The setting key cannot contain whitespace.");
            }

            return trimmed;
        }

        private static void ValidateValue(string? value)
        {
            if (value is null)
            {
                throw new BadRequestException("A setting value is required.");
            }
        }

        private static void ValidateDescription(string? description)
        {
            if (description is { Length: > MaxDescriptionLength })
            {
                throw new BadRequestException(
                    $"The setting description cannot exceed {MaxDescriptionLength} characters.");
            }
        }

        private static PlatformSettingDto Map(PlatformSetting setting) => new()
        {
            Id = setting.Id,
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description,
            IsPublic = setting.IsPublic,
            CreatedAt = setting.CreatedAt,
            UpdatedAt = setting.UpdatedAt
        };
    }
}

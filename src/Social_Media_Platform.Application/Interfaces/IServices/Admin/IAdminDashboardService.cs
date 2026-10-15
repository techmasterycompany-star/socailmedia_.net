using Social_Media_Platform.Application.Common.DTOs.Admin;

namespace Social_Media_Platform.Application.Interfaces.IServices.Admin
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardStatsDto> GetStatsAsync();
    }
}

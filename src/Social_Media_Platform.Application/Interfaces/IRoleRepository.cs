using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(RoleName name, CancellationToken cancellationToken = default);
    }


}

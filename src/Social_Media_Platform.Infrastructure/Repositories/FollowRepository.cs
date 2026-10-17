using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Infrastructure.Data;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly Social_Media_PlatformDbContext _context;

    public FollowRepository(Social_Media_PlatformDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsFollowingAsync(long followerId, long followingId)
    {
        return await _context.Follows.AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId && f.Status == FollowStatus.Accepted);
    }
}
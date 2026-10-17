namespace Social_Media_Platform.Application.Interfaces.IRepositories
{
    public interface IFollowRepository
    {
        Task<bool> IsFollowingAsync(long followerId, long followingId);
    }
}
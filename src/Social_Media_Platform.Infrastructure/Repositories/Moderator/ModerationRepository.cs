using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Interfaces.IRepositories.Moderator;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories.Moderator
{
    public class ModerationRepository : IModerationRepository
    {
        private readonly Social_Media_PlatformDbContext context;
        public ModerationRepository(Social_Media_PlatformDbContext context) => this.context = context;



      

        public async Task<Comment?> GetCommentAsync(long id) => 
            await context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        

        public async Task<LiveStream?> GetStreamAsync(long id) =>
            await context.LiveStreams.FirstOrDefaultAsync(l => l.Id == id);
        

        public async Task<Post?> GetPostAsync(long id) =>
            await context.Posts.FirstOrDefaultAsync(p => p.Id == id);
        

        public async Task UpdatePostAsync(Post post)
        {
            context.Posts.Update(post);
            await context.SaveChangesAsync();
        }

        public async Task UpdateStreamAsync(LiveStream stream)
        {
            context.LiveStreams.Update(stream);
            await context.SaveChangesAsync();
        }
        public async Task DeleteCommentAsync(Comment comment)
        {
            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        public async Task AddLogAsync(ModerationLog log)
        {
            context.ModerationLogs.Add(log);
            await context.SaveChangesAsync();
        }
    }
}

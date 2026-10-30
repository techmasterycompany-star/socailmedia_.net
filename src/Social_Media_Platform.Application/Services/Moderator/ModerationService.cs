using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.DTOS.Moderator;
using Social_Media_Platform.Application.Interfaces.IRepositories.Moderator;
using Social_Media_Platform.Application.Interfaces.IServices.Moderator;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using System.Threading;

namespace Social_Media_Platform.Application.Services.Moderator
{
    public class ModerationService : IModerationService
    {
        private readonly IModerationRepository repo;
        public ModerationService(IModerationRepository repo) => this.repo = repo;




        public async Task<ModerationActionResponse> HidePostAsync(long id, string? reason, long? actorId)
        {
            var actor = RequireActor(actorId);

            var post = await repo.GetPostAsync(id)
                ?? throw new NotFoundException("The requested post could not be found");
            if(post.IsHidden)
                throw new ConflictException("The post is already hidden from public view");

            post.IsHidden = true;
            await repo.UpdatePostAsync(post);

            await LogAsync(actor, ModerationTargetType.Post, post.Id, ModerationAction.Hidden, reason);

            return new ModerationActionResponse
            {
                TargetType = ModerationTargetType.Post,
                TargetId = post.Id,
                Action = ModerationAction.Hidden,
                Message = $"Post {post.Id} has been hidden from public view",
                ExecutedAt = DateTime.UtcNow
            };
        }

        public async Task<ModerationActionResponse> RemoveCommentAsync(long id, string? reason, long? actorId)
        {
            var actor = RequireActor(actorId);

            var comment = await repo.GetCommentAsync(id)
                ?? throw new NotFoundException("The requested comment could not be found.");

            await repo.DeleteCommentAsync(comment);

            await LogAsync(actor, ModerationTargetType.Comment, comment.Id, ModerationAction.Removed, reason);

            return new ModerationActionResponse
            {
                TargetType = ModerationTargetType.Comment,
                TargetId = comment.Id,
                Action = ModerationAction.Removed,
                Message = $"Comment {comment.Id} has been removed",
                ExecutedAt = DateTime.UtcNow
            };
        }

        public async Task<ModerationActionResponse> StopStreamAsync(long id, string? reason, long? actorId)
        {
            var actor = RequireActor(actorId);

            var stream = await repo.GetStreamAsync(id)
                 ?? throw new NotFoundException("The requested live stream could not be found");

            if (stream.Status is LiveStreamStatus.Ended or LiveStreamStatus.Cancelled)
                throw new ConflictException("The live stream is no longer active");
            

            stream.Status = LiveStreamStatus.Cancelled;
            await repo.UpdateStreamAsync(stream);

            await LogAsync(actor, ModerationTargetType.Stream, stream.Id, ModerationAction.Terminated, reason);

            return new ModerationActionResponse
            {
                TargetType = ModerationTargetType.Stream,
                TargetId = stream.Id,
                Action = ModerationAction.Terminated,
                Message = $"Live stream {stream.Id} has been terminated",
                ExecutedAt = DateTime.UtcNow
            };
        }

        private Task LogAsync(
            long actorId,
            ModerationTargetType targetType,
            long targetId,
            ModerationAction action,
            string? reason) =>
            repo.AddLogAsync(new ModerationLog
            {
                ActorId = actorId,
                TargetType = targetType,
                TargetId = targetId,
                Action = action,
                Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim()
            });

        private static long RequireActor(long? actorId) =>
            actorId ?? throw new UnauthorizedAccessException(
                "An authenticated moderator is required to perform this action.");
    }
}

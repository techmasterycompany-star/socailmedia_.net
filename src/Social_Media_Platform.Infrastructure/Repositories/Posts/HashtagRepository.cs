using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Infrastructure.Data;
using Social_Media_Platform.Infrastructure.Helpers;

namespace Social_Media_Platform.Infrastructure.Repositories.Posts
{
    public sealed class HashtagRepository(Social_Media_PlatformDbContext dbContext) : IHashtagRepository
    {
        public async Task AttachHashtagsAsync(Post post, string? text, CancellationToken ct)
        {
            var hashtags = HashtagNormalizer.Extract(text);
            var existingLinks = post.PostHashtags.ToList();

            foreach (var link in existingLinks.Where(link => !hashtags.ContainsKey(link.Hashtag.NormalizedName)))
            {
                dbContext.PostHashtags.Remove(link);
                post.PostHashtags.Remove(link);
            }

            var currentNames = post.PostHashtags
                .Select(link => link.Hashtag.NormalizedName)
                .ToHashSet(StringComparer.Ordinal);

            var missingHashtags = hashtags
                .Where(item => !currentNames.Contains(item.Key))
                .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);

            if (missingHashtags.Count == 0)
                return;

            var existingHashtags = await dbContext.Hashtags
                .Where(hashtag => missingHashtags.Keys.Contains(hashtag.NormalizedName))
                .ToDictionaryAsync(hashtag => hashtag.NormalizedName, ct);

            foreach (var (normalizedName, name) in missingHashtags)
            {
                if (!existingHashtags.TryGetValue(normalizedName, out var hashtag))
                {
                    hashtag = new Hashtag { Name = name, NormalizedName = normalizedName };
                    await dbContext.Hashtags.AddAsync(hashtag, ct);
                    existingHashtags[normalizedName] = hashtag;
                }

                post.PostHashtags.Add(new PostHashtag { Post = post, Hashtag = hashtag });
            }
        }
    }
}

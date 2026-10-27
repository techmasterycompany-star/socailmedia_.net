using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data;

public sealed class Social_Media_PlatformDbContext(DbContextOptions<Social_Media_PlatformDbContext> options)
    : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Profile> Profiles => Set<Profile>();

    public DbSet<Follow> Follows => Set<Follow>();

    public DbSet<Block> Blocks => Set<Block>();

    public DbSet<Post> Posts => Set<Post>();

    public DbSet<PostMedia> PostMedia => Set<PostMedia>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<PostLike> PostLikes => Set<PostLike>();

    public DbSet<LiveStream> LiveStreams => Set<LiveStream>();

    public DbSet<StreamMessage> StreamMessages => Set<StreamMessage>();

    public DbSet<Report> Reports => Set<Report>();
    public DbSet<PlatformSetting> PlatformSettings => Set<PlatformSetting>();

    public DbSet<ModerationLog> ModerationLogs => Set<ModerationLog>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();

    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<SavedPost> SavedPosts => Set<SavedPost>();
    public DbSet<Hashtag> Hashtags => Set<Hashtag>();
    public DbSet<PostHashtag> PostHashtags => Set<PostHashtag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Social_Media_PlatformDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges() => SaveChanges(acceptAllChangesOnSuccess: true);

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditFields();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(entity => entity.CreatedAt).IsModified = false;
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}

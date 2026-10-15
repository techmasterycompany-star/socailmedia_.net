using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class User : SoftDelete
{
    public long RoleId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsVerified { get; set; }

    public UserStatus? Status { get; set; }

    public Role Role { get; set; } = null!;

    public Profile? Profile { get; set; }

    public ICollection<Follow> OutgoingFollows { get; set; } = new List<Follow>();

    public ICollection<Follow> IncomingFollows { get; set; } = new List<Follow>();

    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public ICollection<LiveStream> HostedStreams { get; set; } = new List<LiveStream>();

    public ICollection<StreamMessage> StreamMessages { get; set; } = new List<StreamMessage>();

    public ICollection<Report> SubmittedReports { get; set; } = new List<Report>();

    public ICollection<Report> ReviewedReports { get; set; } = new List<Report>();

    public ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();

    public ICollection<Notification> SentNotifications { get; set; } = new List<Notification>();
}



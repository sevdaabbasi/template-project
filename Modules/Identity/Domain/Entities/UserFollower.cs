using BuildingBlocks.Domain;

namespace Modules.Identity.Domain.Entities;

public class UserFollower : Entity<Guid>
{
    public Guid FollowerId { get; private set; }
    public User Follower { get; private set; } = null!;

    public Guid FollowedId { get; private set; }
    public User Followed { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    private UserFollower() { }

    public UserFollower(Guid followerId, Guid followedId)
    {
        FollowerId = followerId;
        FollowedId = followedId;
        CreatedAt = DateTime.UtcNow;
    }
}

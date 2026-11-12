using BuildingBlocks.Domain;

namespace Modules.Identity.Domain.Entities;

public class UserInterest : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public int InterestId { get; private set; }
    public Interest Interest { get; private set; } = null!;

    private UserInterest() { }

    public UserInterest(Guid userId, int interestId)
    {
        UserId = userId;
        InterestId = interestId;
    }
}

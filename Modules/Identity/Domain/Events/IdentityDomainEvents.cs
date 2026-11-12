using BuildingBlocks.Domain;

namespace Modules.Identity.Domain.Events;

public record UserRegisteredDomainEvent(
    Guid UserId,
    string PhoneNumber,
    string FullName
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record UserOnboardedDomainEvent(
    Guid UserId,
    int CityId
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record UserFollowedDomainEvent(
    Guid FollowerId,
    Guid FollowedId
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record UserUnfollowedDomainEvent(
    Guid FollowerId,
    Guid FollowedId
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

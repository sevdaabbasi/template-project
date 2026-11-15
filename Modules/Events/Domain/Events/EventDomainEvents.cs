using BuildingBlocks.Domain;

namespace Modules.Events.Domain.Events;

public record EventCreatedDomainEvent(
    Guid Id,
    Guid OrganizerId,
    string Title,
    DateTime EventDate
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record EventCancelledDomainEvent(
    Guid Id,
    string Reason
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

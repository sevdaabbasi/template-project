using BuildingBlocks.Domain;

namespace Modules.Events.Domain.Entities;

public class EventParticipant : Entity<Guid>
{
    public Guid EventId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private EventParticipant() { }

    public EventParticipant(Guid eventId, Guid userId)
    {
        EventId = eventId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
    }
}

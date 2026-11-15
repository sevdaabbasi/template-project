using Microsoft.EntityFrameworkCore;
using Modules.Events.Domain.Entities;

namespace Modules.Events.Application.Abstractions;

public interface IEventsDbContext
{
    DbSet<Event> Events { get; }
    DbSet<EventRule> EventRules { get; }
    DbSet<EventParticipant> EventParticipants { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

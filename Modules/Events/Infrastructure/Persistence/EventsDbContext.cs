using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Modules.Events.Application.Abstractions;
using Modules.Events.Domain.Entities;

namespace Modules.Events.Infrastructure.Persistence;

public class EventsDbContext : AppDbContextBase, IEventsDbContext
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventRule> EventRules => Set<EventRule>();
    public DbSet<EventParticipant> EventParticipants => Set<EventParticipant>();

    public EventsDbContext(
        DbContextOptions<EventsDbContext> options,
        IEventDispatcher? eventDispatcher = null) : base(options, eventDispatcher)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("events");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventsDbContext).Assembly);
    }
}

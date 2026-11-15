using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Events.Domain.Entities;

namespace Modules.Events.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events", "events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.LocationUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.LocationName)
            .HasMaxLength(250);

        builder.Property(e => e.Price)
            .HasPrecision(18, 2);

        builder.Property(e => e.GroupChatUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.ExtraNotes)
            .HasMaxLength(2000);

        builder.HasMany(e => e.Rules)
            .WithOne()
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Participants)
            .WithOne()
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.DomainEvents);
    }
}

public class EventRuleConfiguration : IEntityTypeConfiguration<EventRule>
{
    public void Configure(EntityTypeBuilder<EventRule> builder)
    {
        builder.ToTable("EventRules", "events");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RuleText)
            .IsRequired()
            .HasMaxLength(300);
    }
}

public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
{
    public void Configure(EntityTypeBuilder<EventParticipant> builder)
    {
        builder.ToTable("EventParticipants", "events");

        builder.HasKey(ep => ep.Id);

        builder.HasIndex(ep => new { ep.EventId, ep.UserId })
            .IsUnique();
    }
}
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Modules.Events.Application.Abstractions;
using Modules.Events.Application.DTOs;

namespace Modules.Events.Application.Features.Events;

public record JoinEventCommand(Guid EventId, Guid UserId) : ICommand<EventDto>;

public class JoinEventCommandHandler : ICommandHandler<JoinEventCommand, EventDto>
{
    private readonly IEventsDbContext _context;

    public JoinEventCommandHandler(IEventsDbContext context)
    {
        _context = context;
    }

    public async Task<EventDto> Handle(JoinEventCommand request, CancellationToken cancellationToken)
    {
        var ev = await _context.Events
            .Include(e => e.Rules)
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (ev == null)
            throw new NotFoundException("Etkinlik bulunamadı.");

        ev.AddParticipant(request.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return new EventDto(
            ev.Id,
            ev.OrganizerId,
            ev.Title,
            ev.EventDate,
            ev.LocationUrl,
            ev.LocationName,
            ev.CityId,
            ev.InterestId,
            ev.Capacity,
            ev.IsPaid,
            ev.Price,
            ev.GroupChatUrl,
            ev.ExtraNotes,
            ev.Status,
            ev.Rules.Select(r => new EventRuleDto(r.Id, r.RuleText, r.IsCustom, r.DisplayOrder)).ToList(),
            ev.GenerateWhatsAppAnnouncement(),
            ev.Participants.Count,
            true,
            ev.CreatedAt
        );
    }
}

public record LeaveEventCommand(Guid EventId, Guid UserId) : ICommand;

public class LeaveEventCommandHandler : ICommandHandler<LeaveEventCommand>
{
    private readonly IEventsDbContext _context;

    public LeaveEventCommandHandler(IEventsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(LeaveEventCommand request, CancellationToken cancellationToken)
    {
        var ev = await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (ev == null)
            throw new NotFoundException("Etkinlik bulunamadı.");

        ev.RemoveParticipant(request.UserId);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record ParticipantDto(
    Guid UserId,
    DateTime JoinedAt
);

public record GetEventParticipantsQuery(Guid EventId) : IQuery<List<ParticipantDto>>;

public class GetEventParticipantsQueryHandler : IQueryHandler<GetEventParticipantsQuery, List<ParticipantDto>>
{
    private readonly IEventsDbContext _context;

    public GetEventParticipantsQueryHandler(IEventsDbContext context)
    {
        _context = context;
    }

    public async Task<List<ParticipantDto>> Handle(GetEventParticipantsQuery request, CancellationToken cancellationToken)
    {
        var participants = await _context.EventParticipants
            .Where(p => p.EventId == request.EventId)
            .OrderBy(p => p.JoinedAt)
            .Select(p => new ParticipantDto(p.UserId, p.JoinedAt))
            .ToListAsync(cancellationToken);

        return participants;
    }
}

using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Modules.Events.Application.Abstractions;
using Modules.Events.Application.DTOs;

namespace Modules.Events.Application.Features.Events;

public record GetEventByIdQuery(Guid EventId, Guid? CurrentUserId = null) : IQuery<EventDto>;

public class GetEventByIdQueryHandler : IQueryHandler<GetEventByIdQuery, EventDto>
{
    private readonly IEventsDbContext _context;

    public GetEventByIdQueryHandler(IEventsDbContext context)
    {
        _context = context;
    }

    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var ev = await _context.Events
            .Include(e => e.Rules)
            .Include(e => e.Participants)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (ev == null)
            throw new NotFoundException("Etkinlik bulunamadı.");

        var isJoined = request.CurrentUserId.HasValue && ev.Participants.Any(p => p.UserId == request.CurrentUserId.Value);

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
            isJoined,
            ev.CreatedAt
        );
    }
}

using BuildingBlocks.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Modules.Events.Application.Abstractions;
using Modules.Events.Application.DTOs;

namespace Modules.Events.Application.Features.Events;

public record GetEventsQuery(int? CityId = null, int? InterestId = null, Guid? CurrentUserId = null) : IQuery<List<EventDto>>;

public class GetEventsQueryHandler : IQueryHandler<GetEventsQuery, List<EventDto>>
{
    private readonly IEventsDbContext _context;

    public GetEventsQueryHandler(IEventsDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Events
            .Include(e => e.Rules)
            .Include(e => e.Participants)
            .AsNoTracking();

        if (request.CityId.HasValue)
            query = query.Where(e => e.CityId == request.CityId.Value);

        if (request.InterestId.HasValue)
            query = query.Where(e => e.InterestId == request.InterestId.Value);

        var events = await query
            .OrderByDescending(e => e.EventDate)
            .ToListAsync(cancellationToken);

        return events.Select(ev =>
        {
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
        }).ToList();
    }
}

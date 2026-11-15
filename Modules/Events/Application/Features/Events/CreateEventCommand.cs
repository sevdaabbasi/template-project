using BuildingBlocks.Application.Abstractions;
using FluentValidation;
using Modules.Events.Application.Abstractions;
using Modules.Events.Application.DTOs;
using Modules.Events.Domain.Entities;

namespace Modules.Events.Application.Features.Events;

public record CreateEventCommand(
    Guid OrganizerId,
    string Title,
    DateTime EventDate,
    string LocationUrl,
    string? LocationName = null,
    int? CityId = null,
    int? InterestId = null,
    int? Capacity = null,
    bool IsPaid = false,
    decimal? Price = null,
    string? GroupChatUrl = null,
    string? ExtraNotes = null,
    List<CreateEventRuleRequest>? Rules = null
) : ICommand<EventDto>;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.OrganizerId).NotEmpty().WithMessage("Organizatör kimliği zorunludur.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Etkinlik başlığı boş bırakılamaz.").MaximumLength(200);
        RuleFor(x => x.EventDate).GreaterThan(DateTime.UtcNow).WithMessage("Etkinlik tarihi gelecekte bir zaman olmalıdır.");
        RuleFor(x => x.LocationUrl).NotEmpty().WithMessage("Konum linki boş bırakılamaz.").MaximumLength(1000);
        RuleFor(x => x.Capacity).GreaterThan(0).When(x => x.Capacity.HasValue).WithMessage("Kontenjan en az 1 kişi olmalıdır.");
        RuleFor(x => x.Price).GreaterThan(0).When(x => x.IsPaid).WithMessage("Ücretli etkinliklerde fiyat belirtilmelidir.");
    }
}

public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, EventDto>
{
    private readonly IEventsDbContext _context;

    public CreateEventCommandHandler(IEventsDbContext context)
    {
        _context = context;
    }

    public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var ev = Event.Create(
            request.OrganizerId,
            request.Title,
            request.EventDate,
            request.LocationUrl,
            request.LocationName,
            request.CityId,
            request.InterestId,
            request.Capacity,
            request.IsPaid,
            request.Price,
            request.GroupChatUrl,
            request.ExtraNotes);

        if (request.Rules != null && request.Rules.Any())
        {
            foreach (var rule in request.Rules)
            {
                ev.AddRule(rule.RuleText, rule.IsCustom);
            }
        }

        await _context.Events.AddAsync(ev, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var announcementText = ev.GenerateWhatsAppAnnouncement();

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
            announcementText,
            0,
            false,
            ev.CreatedAt
        );
    }
}

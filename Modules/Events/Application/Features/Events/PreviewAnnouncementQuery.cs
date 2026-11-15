using BuildingBlocks.Application.Abstractions;
using FluentValidation;
using Modules.Events.Domain.Entities;

namespace Modules.Events.Application.Features.Events;

public record PreviewAnnouncementQuery(
    string Title,
    DateTime EventDate,
    string? LocationName = null,
    string? LocationUrl = null,
    int? Capacity = null,
    bool IsPaid = false,
    decimal? Price = null,
    List<string>? Rules = null,
    string? ExtraNotes = null,
    string? GroupChatUrl = null
) : IQuery<string>;

public class PreviewAnnouncementQueryValidator : AbstractValidator<PreviewAnnouncementQuery>
{
    public PreviewAnnouncementQueryValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Etkinlik başlığı boş bırakılamaz.");
    }
}

public class PreviewAnnouncementQueryHandler : IQueryHandler<PreviewAnnouncementQuery, string>
{
    public Task<string> Handle(PreviewAnnouncementQuery request, CancellationToken cancellationToken)
    {
        var text = Event.BuildWhatsAppAnnouncement(
            request.Title,
            request.EventDate,
            request.LocationName,
            request.LocationUrl,
            request.Capacity,
            request.IsPaid,
            request.Price,
            request.Rules,
            request.ExtraNotes,
            request.GroupChatUrl
        );

        return Task.FromResult(text);
    }
}

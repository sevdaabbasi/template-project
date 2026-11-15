using Modules.Events.Domain.Enums;

namespace Modules.Events.Application.DTOs;

public record EventDto(
    Guid Id,
    Guid OrganizerId,
    string Title,
    DateTime EventDate,
    string LocationUrl,
    string? LocationName,
    int? CityId,
    int? InterestId,
    int? Capacity,
    bool IsPaid,
    decimal? Price,
    string? GroupChatUrl,
    string? ExtraNotes,
    EventStatus Status,
    List<EventRuleDto> Rules,
    string AnnouncementText,
    int ParticipantsCount,
    bool IsUserJoined,
    DateTime CreatedAt
);

public record EventRuleDto(
    Guid Id,
    string RuleText,
    bool IsCustom,
    int DisplayOrder
);

public record CreateEventRuleRequest(
    string RuleText,
    bool IsCustom = false
);

public record PreviewAnnouncementRequest(
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
);

public record CancelEventRequest(string Reason);

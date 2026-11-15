using BuildingBlocks.Application.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Events.Application.DTOs;
using Modules.Events.Application.Features.Events;

namespace Modules.Events.Api.Controllers;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly ISender _sender;

    public EventsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Yeni bir etkinlik ve WhatsApp duyuru şablonu oluşturur.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EventDto>> CreateEvent(
        [FromBody] CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();

        var command = new CreateEventCommand(
            currentUserId,
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
            request.ExtraNotes,
            request.Rules
        );

        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Henüz veritabanına kaydetmeden canlı WhatsApp duyuru önizlemesi üretir.
    /// </summary>
    [HttpPost("preview")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> PreviewAnnouncement(
        [FromBody] PreviewAnnouncementRequest request,
        CancellationToken cancellationToken)
    {
        var query = new PreviewAnnouncementQuery(
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

        var announcementText = await _sender.Send(query, cancellationToken);
        return Ok(new { announcementText });
    }

    /// <summary>
    /// Etkinlikleri şehir veya ilgi alanına göre filtreleyerek listeler.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<EventDto>>> GetEvents(
        [FromQuery] int? cityId,
        [FromQuery] int? interestId,
        CancellationToken cancellationToken)
    {
        Guid? currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        var query = new GetEventsQuery(cityId, interestId, currentUserId);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen etkinliğin detaylarını, kurallarını ve katılımcı sayısını getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventDto>> GetEventById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        Guid? currentUserId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        var query = new GetEventByIdQuery(id, currentUserId);
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Etkinliği iptal eder (Yalnızca oluşturan organizatör yapabilir).
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelEvent(
        [FromRoute] Guid id,
        [FromBody] CancelEventRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        await _sender.Send(new CancelEventCommand(id, currentUserId, request.Reason), cancellationToken);
        return Ok(new { message = "Etkinlik başarıyla iptal edildi." });
    }

    /// <summary>
    /// Etkinliğe katılım sağlar.
    /// </summary>
    [HttpPost("{id:guid}/join")]
    public async Task<ActionResult<EventDto>> JoinEvent(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var result = await _sender.Send(new JoinEventCommand(id, currentUserId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Etkinlik katılımını iptal eder (etkinlikten ayrılır).
    /// </summary>
    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> LeaveEvent(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        await _sender.Send(new LeaveEventCommand(id, currentUserId), cancellationToken);
        return Ok(new { message = "Etkinlikten ayrıldınız." });
    }

    /// <summary>
    /// Etkinliğin katılımcı listesini getirir.
    /// </summary>
    [HttpGet("{id:guid}/participants")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ParticipantDto>>> GetParticipants(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEventParticipantsQuery(id), cancellationToken);
        return Ok(result);
    }
}

public record CreateEventRequest(
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
);

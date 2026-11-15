using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Modules.Events.Application.Abstractions;

namespace Modules.Events.Application.Features.Events;

public record CancelEventCommand(Guid EventId, Guid UserId, string Reason) : ICommand;

public class CancelEventCommandHandler : ICommandHandler<CancelEventCommand>
{
    private readonly IEventsDbContext _context;

    public CancelEventCommandHandler(IEventsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CancelEventCommand request, CancellationToken cancellationToken)
    {
        var ev = await _context.Events
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken);

        if (ev == null)
            throw new NotFoundException("Etkinlik bulunamadı.");

        if (ev.OrganizerId != request.UserId)
            throw new InvalidOperationException("Yalnızca etkinliği oluşturan organizatör iptal edebilir.");

        ev.Cancel(request.Reason);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

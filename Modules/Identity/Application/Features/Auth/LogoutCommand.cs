using BuildingBlocks.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions;

namespace Modules.Identity.Application.Features.Auth;

public record LogoutCommand(string AccessToken, string RefreshToken) : ICommand;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IIdentityDbContext _context;
    private readonly ITokenBlacklistService _blacklistService;

    public LogoutCommandHandler(IIdentityDbContext context, ITokenBlacklistService blacklistService)
    {
        _context = context;
        _blacklistService = blacklistService;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.AccessToken))
            await _blacklistService.BlacklistTokenAsync(request.AccessToken, TimeSpan.FromHours(24));

        if (!string.IsNullOrEmpty(request.RefreshToken))
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == request.RefreshToken, cancellationToken);

            token?.Revoke();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}


using System.Security.Claims;
using BuildingBlocks.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Application.Common;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Domain.Entities;
using static BuildingBlocks.Infrastructure.Exceptions.GlobalExceptionHandler;

namespace Modules.Identity.Application.Features.Auth;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<AuthResponseDto>;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IIdentityDbContext _context;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IIdentityDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
            throw new InvalidCredentialsException();

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidCredentialsException();

        var user = await _context.Users
            .Include(u => u.City)
            .Include(u => u.UserInterests).ThenInclude(ui => ui.Interest)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || !user.IsActive)
            throw new InvalidCredentialsException();

        var existingToken = user.RefreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken && x.IsActive);
        if (existingToken is null)
            throw new InvalidCredentialsException();

        user.RevokeRefreshToken(request.RefreshToken);

        var (accessToken, newRefreshToken, expiresAt) = _tokenService.GenerateTokens(user);
        user.AddRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(30));

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(accessToken, newRefreshToken, expiresAt, UserMapper.ToDto(user));
    }
}

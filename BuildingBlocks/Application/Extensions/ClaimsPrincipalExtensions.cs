using System.Security.Claims;

namespace BuildingBlocks.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guid))
        {
            throw new UnauthorizedAccessException("Geçersiz kullanıcı kimliği.");
        }

        return guid;
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst(ClaimTypes.Email)?.Value;
    }
}

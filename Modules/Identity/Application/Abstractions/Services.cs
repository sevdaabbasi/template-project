using System.Security.Claims;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Domain.Entities;

namespace Modules.Identity.Application.Abstractions;

public interface ITokenService
{
    (string AccessToken, string RefreshToken, DateTime ExpiresAt) GenerateTokens(User user);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

public interface ISmsOtpService
{
    Task<string> SendOtpAsync(string phoneNumber, string purpose, CancellationToken cancellationToken = default);
    Task<bool> VerifyOtpAsync(string phoneNumber, string code, string purpose, CancellationToken cancellationToken = default);
}

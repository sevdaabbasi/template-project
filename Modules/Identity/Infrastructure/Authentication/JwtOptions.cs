namespace Modules.Identity.Infrastructure.Authentication;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; set; } = "OrganicCommunitySecretKey_SuperSecureKeyForJwtTokens2026!#";
    public string Issuer { get; set; } = "CSMobil.Backend";
    public string Audience { get; set; } = "CSMobil.App";
    public int ExpiryMinutes { get; set; } = 1440; // 24 hours
    public int RefreshTokenExpiryDays { get; set; } = 30;
}

namespace Modules.Identity.Application.DTOs;

public record CityDto(
    int Id,
    string Name,
    int PlateCode
);

public record InterestDto(
    int Id,
    string Name,
    string IconName
);

public record UserFollowerDto(
    Guid Id,
    string FullName,
    string? AvatarUrl,
    string? Bio,
    bool IsFollowing
);

public record ExternalAuthUserDto(
    string Provider,
    string ProviderId,
    string FullName,
    string? Email
);

using Modules.Identity.Domain.Enums;

namespace Modules.Identity.Application.DTOs;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

public record SendOtpResponseDto(
    string PhoneNumber,
    string Message,
    string? DevCode = null
);

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string PhoneNumber,
    string Email,
    DateOnly? DateOfBirth,
    string Gender,
    string Role,
    int? CityId,
    string? CityName,
    string? Bio,
    string? AvatarUrl,
    bool IsCsPlus,
    bool IsOnboarded,
    int FollowersCount,
    int FollowingCount,
    int EventsAttendedCount,
    List<string> Interests
);


public record PendingRegistrationDraft(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    DateOnly DateOfBirth,
    Gender Gender
);
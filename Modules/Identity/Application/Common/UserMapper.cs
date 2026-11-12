using Modules.Identity.Application.DTOs;
using Modules.Identity.Domain.Entities;

namespace Modules.Identity.Application.Common;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto(
            Id: user.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            FullName: user.FullName,
            PhoneNumber: user.PhoneNumber,
            Email: user.Email,
            DateOfBirth: user.DateOfBirth,
            Gender: user.Gender.ToString(),
            Role: user.Role.ToString(),
            CityId: user.CityId,
            CityName: user.City?.Name,
            Bio: user.Bio,
            AvatarUrl: user.AvatarUrl,
            IsCsPlus: user.IsCsPlus,
            IsOnboarded: user.IsOnboarded,
            FollowersCount: user.FollowersCount,
            FollowingCount: user.FollowingCount,
            EventsAttendedCount: user.EventsAttendedCount,
            Interests: user.UserInterests?.Select(x => x.Interest?.Name ?? string.Empty).Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>()
        );
    }
}

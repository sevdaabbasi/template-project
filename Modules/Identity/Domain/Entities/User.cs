using BuildingBlocks.Domain;
using Modules.Identity.Domain.Enums;
using Modules.Identity.Domain.Events;

namespace Modules.Identity.Domain.Entities;

public class User : AggregateRoot<Guid>
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateOnly? DateOfBirth { get; private set; }
    public Gender Gender { get; private set; } = Gender.Unspecified;

    public UserRole Role { get; private set; } = UserRole.Member;
    public bool IsTermsAccepted { get; private set; } = true;
    public DateTime? TermsAcceptedAt { get; private set; }
    public bool IsOnboarded { get; private set; }
    public int? CityId { get; private set; }
    public City? City { get; private set; }
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }
    public bool IsCsPlus { get; private set; }
    public int FollowersCount { get; private set; }
    public int FollowingCount { get; private set; }
    public int EventsAttendedCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<UserInterest> _userInterests = new();
    public IReadOnlyCollection<UserInterest> UserInterests => _userInterests.AsReadOnly();

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private readonly List<UserFollower> _followers = new();
    public IReadOnlyCollection<UserFollower> Followers => _followers.AsReadOnly();

    private readonly List<UserFollower> _following = new();
    public IReadOnlyCollection<UserFollower> Following => _following.AsReadOnly();

    private User() { }

    public static User Create(
        string firstName,
        string lastName,
        string phoneNumber,
        string email,
        DateOnly dateOfBirth,
        Gender gender)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PhoneNumber = NormalizePhoneNumber(phoneNumber),
            Email = email.Trim().ToLowerInvariant(),
            DateOfBirth = dateOfBirth,
            Gender = gender,
            Role = UserRole.Member,
            IsTermsAccepted = true,
            TermsAcceptedAt = DateTime.UtcNow,
            IsOnboarded = false,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id, user.PhoneNumber, user.FullName));
        return user;
    }

    public void CompleteOnboarding(int cityId, IEnumerable<int> interestIds)
    {
        CityId = cityId;
        IsOnboarded = true;
        UpdatedAt = DateTime.UtcNow;

        _userInterests.Clear();
        foreach (var interestId in interestIds.Distinct())
        {
            _userInterests.Add(new UserInterest(Id, interestId));
        }

        AddDomainEvent(new UserOnboardedDomainEvent(Id, cityId));
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        string? bio,
        DateOnly? dateOfBirth = null,
        Gender? gender = null)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Bio = bio?.Trim();
        if (dateOfBirth.HasValue) DateOfBirth = dateOfBirth.Value;
        if (gender.HasValue) Gender = gender.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvatar(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRefreshToken(string token, DateTime expiresAt)
    {
        _refreshTokens.Add(new RefreshToken(Id, token, expiresAt));
    }

    public void RevokeRefreshToken(string token)
    {
        var existingToken = _refreshTokens.FirstOrDefault(x => x.Token == token);
        existingToken?.Revoke();
    }

    public void SetCsPlus(bool isCsPlus)
    {
        IsCsPlus = isCsPlus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementFollowers() => FollowersCount++;
    public void DecrementFollowers() => FollowersCount = Math.Max(0, FollowersCount - 1);
    public void IncrementFollowing() => FollowingCount++;
    public void DecrementFollowing() => FollowingCount = Math.Max(0, FollowingCount - 1);
    public void IncrementEventsAttended() => EventsAttendedCount++;

    public static string NormalizePhoneNumber(string phone)
    {
        var cleaned = new string(phone.Where(char.IsDigit).ToArray());
        if (cleaned.StartsWith("90") && cleaned.Length == 12)
        {
            return "+" + cleaned;
        }
        if (cleaned.StartsWith("0") && cleaned.Length == 11)
        {
            return "+9" + cleaned;
        }
        if (cleaned.Length == 10)
        {
            return "+90" + cleaned;
        }
        return phone.Trim();
    }
}

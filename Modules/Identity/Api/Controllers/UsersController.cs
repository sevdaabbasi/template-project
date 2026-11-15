using BuildingBlocks.Application.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Application.Features.Users;
using Modules.Identity.Domain.Enums;

namespace Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// CS Mobil Hoş Geldiniz: Şehir ve ilgi alanları (Doğa, Kültür, Spor, Sanat) seçimi ile onboarding tamamlanır.
    /// </summary>
    [HttpPost("onboarding")]
    public async Task<ActionResult<UserDto>> CompleteOnboarding(
        [FromBody] OnboardingRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var command = new CompleteOnboardingCommand(currentUserId, request.CityId, request.InterestIds);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Giriş yapan kullanıcının kendi profil bilgilerini getirir.
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMyProfile(CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var result = await _sender.Send(new GetMyProfileQuery(currentUserId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Profil bilgilerini (Ad, Soyad, Biyografi, Doğum Tarihi, Cinsiyet) günceller.
    /// </summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var command = new UpdateProfileCommand(
            currentUserId,
            request.FirstName,
            request.LastName,
            request.Bio,
            request.DateOfBirth,
            request.Gender);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Profil fotoğrafını günceller.
    /// </summary>
    [HttpPost("me/avatar")]
    public async Task<ActionResult<object>> UpdateAvatar(
        [FromBody] UpdateAvatarRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var command = new UpdateAvatarCommand(currentUserId, request.AvatarUrl);
        var avatarUrl = await _sender.Send(command, cancellationToken);
        return Ok(new { avatarUrl });
    }

    /// <summary>
    /// Başka bir kullanıcının profilini, rozet ve takipçi istatistiklerini getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUserProfile(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserProfileByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen kullanıcıyı takibe alır.
    /// </summary>
    [HttpPost("{id:guid}/follow")]
    public async Task<IActionResult> FollowUser(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var result = await _sender.Send(new FollowUserCommand(currentUserId, id), cancellationToken);
        return Ok(new { success = result, message = "Kullanıcı takibe alındı." });
    }

    /// <summary>
    /// Belirtilen kullanıcıyı takipten çıkarır.
    /// </summary>
    [HttpDelete("{id:guid}/follow")]
    public async Task<IActionResult> UnfollowUser(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var result = await _sender.Send(new UnfollowUserCommand(currentUserId, id), cancellationToken);
        return Ok(new { success = result, message = "Takipten çıkarıldı." });
    }

    /// <summary>
    /// Kullanıcının takipçilerini listeler.
    /// </summary>
    [HttpGet("{id:guid}/followers")]
    public async Task<ActionResult<List<UserFollowerDto>>> GetFollowers(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var result = await _sender.Send(new GetFollowersQuery(id, currentUserId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcının takip ettiklerini listeler.
    /// </summary>
    [HttpGet("{id:guid}/following")]
    public async Task<ActionResult<List<UserFollowerDto>>> GetFollowing(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var result = await _sender.Send(new GetFollowingQuery(id, currentUserId), cancellationToken);
        return Ok(result);
    }
}

public record OnboardingRequest(int CityId, List<int> InterestIds);
public record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string? Bio,
    DateOnly? DateOfBirth = null,
    Gender? Gender = null);
public record UpdateAvatarRequest(string AvatarUrl);


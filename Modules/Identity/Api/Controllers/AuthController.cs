using BuildingBlocks.Application.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Application.Features.Auth;

namespace Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// 1. Adım: Kullanıcı kayıt formunu doldurup 'Devam Et' dediğinde çağrılır. 
    /// Bilgiler doğrulanır ve SMS kodu gönderilir. (Veritabanına kullanıcı HENÜZ eklenmez!)
    /// </summary>
    [HttpPost("register/request-otp")]
    [AllowAnonymous]
    public async Task<ActionResult<SendOtpResponseDto>> RequestRegisterOtp(
        [FromBody] RequestRegisterOtpCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// 2. Adım: Kullanıcı 6 haneli SMS kodunu girdiğinde çağrılır.
    /// Kod doğrulanır ve kullanıcı İLK KEZ burada veritabanına kaydedilir.
    /// </summary>
    [HttpPost("register/confirm")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> ConfirmRegister(
        [FromBody] ConfirmRegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Giriş ekranında telefon numarasına giriş SMS onay kodu gönderir.
    /// </summary>
    [HttpPost("login/request-otp")]
    [AllowAnonymous]
    public async Task<ActionResult<SendOtpResponseDto>> RequestLoginOtp(
        [FromBody] RequestLoginOtpCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Telefon numarası ve SMS onay kodu ile şifresiz giriş yapar.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Süresi dolan Access Token'ı yenilemek için kullanılır.
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Oturumu sonlandırır, mevcut access token'ı kara listeye (blacklist) alır.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var accessToken = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length).Trim() : string.Empty;

        await _sender.Send(new LogoutCommand(accessToken, request.RefreshToken), cancellationToken);
        return Ok(new { message = "Başarıyla çıkış yapıldı." });
    }
}

public record LogoutRequest(string RefreshToken);

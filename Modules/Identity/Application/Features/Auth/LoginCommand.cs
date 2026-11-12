using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Application.Common;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Domain.Entities;
using static BuildingBlocks.Infrastructure.Exceptions.GlobalExceptionHandler;
using ValidationException = BuildingBlocks.Domain.Exceptions.ValidationException;

namespace Modules.Identity.Application.Features.Auth;

public record LoginCommand(string PhoneNumber, string Code) : ICommand<AuthResponseDto>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası zorunludur.");
        RuleFor(x => x.Code).NotEmpty().WithMessage("SMS doğrulama kodu zorunludur.").Length(6).WithMessage("Kod 6 haneli olmalıdır.");
    }
}

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityDbContext _context;
    private readonly ISmsOtpService _smsOtpService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IIdentityDbContext context, ISmsOtpService smsOtpService, ITokenService tokenService)
    {
        _context = context;
        _smsOtpService = smsOtpService;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedPhone = User.NormalizePhoneNumber(request.PhoneNumber);

        var isOtpValid = await _smsOtpService.VerifyOtpAsync(normalizedPhone, request.Code.Trim(), "Login", cancellationToken);
        if (!isOtpValid)
            throw new ValidationException("SMS doğrulama kodu geçersiz veya süresi dolmuş.");

        var user = await _context.Users
            .Include(u => u.City)
            .Include(u => u.UserInterests).ThenInclude(ui => ui.Interest)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.PhoneNumber == normalizedPhone, cancellationToken);

        if (user is null)
            throw new NotFoundException("Kullanıcı bulunamadı. Lütfen önce kayıt olun.");

        if (!user.IsActive)
            throw new InvalidOperationException("Hesabınız devre dışı bırakılmıştır.");

        var (accessToken, refreshToken, expiresAt) = _tokenService.GenerateTokens(user);
        user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(30));

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(accessToken, refreshToken, expiresAt, UserMapper.ToDto(user));
    }
}


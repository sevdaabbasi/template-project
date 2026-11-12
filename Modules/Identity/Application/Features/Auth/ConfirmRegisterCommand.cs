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

public record ConfirmRegisterCommand(
    string PhoneNumber,
    string Code
) : ICommand<AuthResponseDto>;

public class ConfirmRegisterCommandValidator : AbstractValidator<ConfirmRegisterCommand>
{
    public ConfirmRegisterCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası zorunludur.");
        RuleFor(x => x.Code).NotEmpty().WithMessage("SMS doğrulama kodu zorunludur.").Length(6).WithMessage("Kod 6 haneli olmalıdır.");
    }
}

public class ConfirmRegisterCommandHandler : ICommandHandler<ConfirmRegisterCommand, AuthResponseDto>
{
    private readonly IIdentityDbContext _context;
    private readonly ISmsOtpService _smsOtpService;
    private readonly ITokenService _tokenService;
    private readonly ICacheService _cache;

    public ConfirmRegisterCommandHandler(
        IIdentityDbContext context,
        ISmsOtpService smsOtpService,
        ITokenService tokenService,
        ICacheService cache)
    {
        _context = context;
        _smsOtpService = smsOtpService;
        _tokenService = tokenService;
        _cache = cache;
    }

    public async Task<AuthResponseDto> Handle(ConfirmRegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedPhone = User.NormalizePhoneNumber(request.PhoneNumber);
        var cacheKey = $"pending_reg:{normalizedPhone}";

        var isOtpValid = await _smsOtpService.VerifyOtpAsync(normalizedPhone, request.Code.Trim(), "Register", cancellationToken);
        if (!isOtpValid)
            throw new ValidationException("SMS doğrulama kodu geçersiz veya süresi dolmuş.");

        var draft = await _cache.GetAsync<PendingRegistrationDraft>(cacheKey, cancellationToken);
        if (draft is null)
            throw new ValidationException("Kayıt oturumunuzun süresi dolmuş. Lütfen kayıt formunu tekrar doldurunuz.");

        var phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == normalizedPhone, cancellationToken);
        if (phoneExists)
            throw new AlreadyExistsException("Bu telefon numarası ile kayıtlı bir hesap zaten var.");

        var user = User.Create(draft.FirstName, draft.LastName, draft.PhoneNumber, draft.Email, draft.DateOfBirth, draft.Gender);

        var (accessToken, refreshToken, expiresAt) = _tokenService.GenerateTokens(user);
        user.AddRefreshToken(refreshToken, DateTime.UtcNow.AddDays(30));

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(cacheKey, cancellationToken);

        return new AuthResponseDto(accessToken, refreshToken, expiresAt, UserMapper.ToDto(user));
    }
}


using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Domain.Entities;
using Modules.Identity.Domain.Enums;
using static BuildingBlocks.Infrastructure.Exceptions.GlobalExceptionHandler;

namespace Modules.Identity.Application.Features.Auth;

public record RequestRegisterOtpCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    DateOnly DateOfBirth,
    Gender Gender
) : ICommand<SendOtpResponseDto>;

public class RequestRegisterOtpCommandValidator : AbstractValidator<RequestRegisterOtpCommand>
{
    public RequestRegisterOtpCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad zorunludur.").MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad zorunludur.").MaximumLength(50);
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası zorunludur.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta adresi zorunludur.").EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
        RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("Doğum tarihi zorunludur.")
            .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Doğum tarihi gelecekte olamaz.");
        RuleFor(x => x.Gender).IsInEnum().WithMessage("Geçerli bir cinsiyet seçiniz.");
    }
}

public class RequestRegisterOtpCommandHandler : ICommandHandler<RequestRegisterOtpCommand, SendOtpResponseDto>
{
    private readonly IIdentityDbContext _context;
    private readonly ISmsOtpService _smsOtpService;
    private readonly ICacheService _cache;

    public RequestRegisterOtpCommandHandler(
        IIdentityDbContext context,
        ISmsOtpService smsOtpService,
        ICacheService cache)
    {
        _context = context;
        _smsOtpService = smsOtpService;
        _cache = cache;
    }

    public async Task<SendOtpResponseDto> Handle(RequestRegisterOtpCommand request, CancellationToken cancellationToken)
    {
        var normalizedPhone = User.NormalizePhoneNumber(request.PhoneNumber);
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == normalizedPhone, cancellationToken);
        if (phoneExists)
            throw new AlreadyExistsException("Bu telefon numarası ile kayıtlı bir hesap zaten var. Lütfen giriş yapın.");

        var emailExists = await _context.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (emailExists)
            throw new AlreadyExistsException("Bu e-posta adresi ile kayıtlı bir hesap zaten var.");

        var draft = new PendingRegistrationDraft(
            request.FirstName,
            request.LastName,
            normalizedPhone,
            normalizedEmail,
            request.DateOfBirth,
            request.Gender);

        await _cache.SetAsync($"pending_reg:{normalizedPhone}", draft, TimeSpan.FromMinutes(5), cancellationToken);

        var code = await _smsOtpService.SendOtpAsync(normalizedPhone, "Register", cancellationToken);

        return new SendOtpResponseDto(
            PhoneNumber: normalizedPhone,
            Message: "Kayıt bilgileri doğrulandı. SMS onay kodu telefonunuza gönderildi.",
            DevCode: code);
    }
}


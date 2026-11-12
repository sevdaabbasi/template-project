using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Domain.Entities;
using static BuildingBlocks.Infrastructure.Exceptions.GlobalExceptionHandler;

namespace Modules.Identity.Application.Features.Auth;

public record RequestLoginOtpCommand(string PhoneNumber) : ICommand<SendOtpResponseDto>;

public class RequestLoginOtpCommandValidator : AbstractValidator<RequestLoginOtpCommand>
{
    public RequestLoginOtpCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası zorunludur.");
    }
}

public class RequestLoginOtpCommandHandler : ICommandHandler<RequestLoginOtpCommand, SendOtpResponseDto>
{
    private readonly IIdentityDbContext _context;
    private readonly ISmsOtpService _smsOtpService;

    public RequestLoginOtpCommandHandler(IIdentityDbContext context, ISmsOtpService smsOtpService)
    {
        _context = context;
        _smsOtpService = smsOtpService;
    }

    public async Task<SendOtpResponseDto> Handle(RequestLoginOtpCommand request, CancellationToken cancellationToken)
    {
        var normalizedPhone = User.NormalizePhoneNumber(request.PhoneNumber);

        var userExists = await _context.Users.AnyAsync(u => u.PhoneNumber == normalizedPhone, cancellationToken);
        if (!userExists)
            throw new NotFoundException("Bu telefon numarasına ait bir hesap bulunamadı. Lütfen önce kayıt olun.");

        var code = await _smsOtpService.SendOtpAsync(normalizedPhone, "Login", cancellationToken);

        return new SendOtpResponseDto(
            PhoneNumber: normalizedPhone,
            Message: "Giriş için SMS onay kodu telefonunuza gönderildi.",
            DevCode: code);
    }
}


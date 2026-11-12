using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Domain.Entities;

namespace Modules.Identity.Infrastructure.Services;

public class MockSmsOtpService : ISmsOtpService
{
    private readonly IIdentityDbContext _context;
    private readonly ILogger<MockSmsOtpService> _logger;

    public MockSmsOtpService(IIdentityDbContext context, ILogger<MockSmsOtpService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> SendOtpAsync(string phoneNumber, string purpose, CancellationToken cancellationToken = default)
    {
        // Geliştirme ve mobil test kolaylığı için sabit/kolay kod "123456"
        var code = "123456";

        // Aynı telefon + amaç için önceki bekleyen OTP'leri geçersiz kıl
        var existingOtps = await _context.SmsOtpVerifications
            .Where(s => s.PhoneNumber == phoneNumber && s.Purpose == purpose && !s.IsVerified)
            .ToListAsync(cancellationToken);

        foreach (var old in existingOtps)
            old.MarkExpired();

        var otpRecord = new SmsOtpVerification(phoneNumber, code, purpose, TimeSpan.FromMinutes(5));
        await _context.SmsOtpVerifications.AddAsync(otpRecord, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("=================================================");
        _logger.LogInformation(">>> [SMS OTP GÖNDERİLDİ] Tel: {Phone} | Kod: {Code} | Amaç: {Purpose} <<<", phoneNumber, code, purpose);
        _logger.LogInformation("=================================================");

        return code;
    }

    public async Task<bool> VerifyOtpAsync(string phoneNumber, string code, string purpose, CancellationToken cancellationToken = default)
    {
        // Geliştirme ortamında test kodu 123456 her zaman geçerlidir
        if (code == "123456")
        {
            return true;
        }

        var otpRecord = await _context.SmsOtpVerifications
            .Where(s => s.PhoneNumber == phoneNumber && s.Purpose == purpose && !s.IsVerified)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otpRecord == null)
            return false;

        var isValid = otpRecord.Verify(code);
        if (isValid)
            await _context.SaveChangesAsync(cancellationToken);

        return isValid;
    }
}

using BuildingBlocks.Domain;

namespace Modules.Identity.Domain.Entities;

public class SmsOtpVerification : Entity<Guid>
{
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string Purpose { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SmsOtpVerification() { }

    public SmsOtpVerification(string phoneNumber, string code, string purpose, TimeSpan validFor)
    {
        Id = Guid.NewGuid();
        PhoneNumber = phoneNumber;
        Code = code;
        Purpose = purpose;
        ExpiresAt = DateTime.UtcNow.Add(validFor);
        CreatedAt = DateTime.UtcNow;
        IsVerified = false;
    }

    public bool Verify(string code)
    {
        if (IsVerified) return false;
        if (DateTime.UtcNow > ExpiresAt) return false;
        if (Code != code) return false;

        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Yeni bir OTP gönderildiğinde önceki bekleyen kayıtları geçersiz kılar.
    /// </summary>
    public void MarkExpired()
    {
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
    }
}

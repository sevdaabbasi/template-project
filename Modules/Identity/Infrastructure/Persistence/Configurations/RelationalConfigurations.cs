using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Identity.Domain.Entities;

namespace Modules.Identity.Infrastructure.Persistence.Configurations;

public class UserInterestConfiguration : IEntityTypeConfiguration<UserInterest>
{
    public void Configure(EntityTypeBuilder<UserInterest> builder)
    {
        builder.ToTable("UserInterests", "identity");

        builder.HasKey(ui => ui.Id);

        builder.HasIndex(ui => new { ui.UserId, ui.InterestId })
            .IsUnique();

        builder.HasOne(ui => ui.Interest)
            .WithMany()
            .HasForeignKey(ui => ui.InterestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "identity");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(rt => rt.Token)
            .IsUnique();
    }
}

public class UserFollowerConfiguration : IEntityTypeConfiguration<UserFollower>
{
    public void Configure(EntityTypeBuilder<UserFollower> builder)
    {
        builder.ToTable("UserFollowers", "identity");

        builder.HasKey(uf => uf.Id);

        builder.HasIndex(uf => new { uf.FollowerId, uf.FollowedId })
            .IsUnique();
    }
}

public class SmsOtpVerificationConfiguration : IEntityTypeConfiguration<SmsOtpVerification>
{
    public void Configure(EntityTypeBuilder<SmsOtpVerification> builder)
    {
        builder.ToTable("SmsOtpVerifications", "identity");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.Purpose)
            .IsRequired()
            .HasMaxLength(50);
    }
}

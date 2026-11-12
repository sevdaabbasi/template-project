using Microsoft.EntityFrameworkCore;
using Modules.Identity.Domain.Entities;

namespace Modules.Identity.Application.Abstractions;

public interface IIdentityDbContext
{
    DbSet<User> Users { get; }
    DbSet<City> Cities { get; }
    DbSet<Interest> Interests { get; }
    DbSet<UserInterest> UserInterests { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<UserFollower> UserFollowers { get; }
    DbSet<SmsOtpVerification> SmsOtpVerifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

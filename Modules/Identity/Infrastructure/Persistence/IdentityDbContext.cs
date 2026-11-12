using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Domain.Entities;

namespace Modules.Identity.Infrastructure.Persistence;

public class IdentityDbContext : AppDbContextBase, IIdentityDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<UserInterest> UserInterests => Set<UserInterest>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserFollower> UserFollowers => Set<UserFollower>();
    public DbSet<SmsOtpVerification> SmsOtpVerifications => Set<SmsOtpVerification>();

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        IEventDispatcher? eventDispatcher = null) : base(options, eventDispatcher)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}

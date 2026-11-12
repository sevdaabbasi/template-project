using System.Text;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Data;
using BuildingBlocks.Infrastructure.Caching;
using BuildingBlocks.Infrastructure.Modules;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Infrastructure.Authentication;
using Modules.Identity.Infrastructure.Persistence;
using Modules.Identity.Infrastructure.Services;

namespace Modules.Identity.Infrastructure;

public class IdentityModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // 1. JWT Ayarları
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        services.Configure<JwtOptions>(jwtSection);
        var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // 2. Veritabanı (PostgreSQL - Npgsql)
        var connectionString = configuration.GetConnectionString("Database") 
            ?? "Host=localhost;Port=5432;Database=cs_mobil_db;Username=postgres;Password=postgres";

        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
            });
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            options.InstanceName = "CSMobil:";
        });

        services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());

        // 3. Servisler (Şifresiz SMS OTP, JWT & Dağıtık Önbellek)
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ISmsOtpService, MockSmsOtpService>();
        services.AddScoped<ICacheService, DistributedCacheService>();

        // 4. MediatR & FluentValidation (Application katmanı handler ve validatörleri)
        var appAssembly = typeof(Modules.Identity.Application.DTOs.AuthResponseDto).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(appAssembly));
        services.AddValidatorsFromAssembly(appAssembly);
    }

    public void UseModule(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        try
        {
            db.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<Microsoft.Extensions.Logging.ILogger<IdentityModule>>();
            logger?.LogError(ex, "PostgreSQL veritabanı başlatılırken hata oluştu: {Message}", ex.Message);
        }
    }
}

using BuildingBlocks.Application.Data;
using BuildingBlocks.Infrastructure.Modules;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Events.Application.Abstractions;
using Modules.Events.Infrastructure.Persistence;

namespace Modules.Events.Infrastructure;

public class EventsModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? "Host=localhost;Port=5432;Database=cs_mobil_db;Username=postgres;Password=postgres";

        services.AddDbContext<EventsDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "events");
            });
        });

        services.AddScoped<IEventsDbContext>(sp => sp.GetRequiredService<EventsDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EventsDbContext>());

        // Application assembly'sini tarayıp Handler ve Validator'ları kaydet
        var appAssembly = typeof(IEventsDbContext).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(appAssembly));
        services.AddValidatorsFromAssembly(appAssembly);
    }

    public void UseModule(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
        try
        {
            db.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<EventsModule>>();
            logger?.LogError(ex, "Events modülü veritabanı başlatılırken hata oluştu: {Message}", ex.Message);
        }
    }
}
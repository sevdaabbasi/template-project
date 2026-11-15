using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Infrastructure.Authentication;
using BuildingBlocks.Infrastructure.Exceptions;
using BuildingBlocks.Infrastructure.Logging;
using BuildingBlocks.Infrastructure.Modules;
using MediatR;
using Microsoft.OpenApi.Models;
using Modules.Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Temel Servisler & WebApi
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.ProducesAttribute("application/json"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

// 2. Token Blacklist & Hata Yönetimi Altyapısı
builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 3. MediatR Pipeline Davranışları
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// 4. Swagger Yapılandırması (JWT Bearer Destekli)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CS Mobil API",
        Version = "v1",
        Description = "CS Mobil - Doğa ve Topluluk Deneyimi Platformu Backend Servisleri"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Access Token'ınızı buraya girin (Örn: 'ey...') "
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 5. Modüllerin Otomatik Keşfi ve Kaydı (IdentityModule, EventsModule vb.)
builder.Services.AddModules(
    builder.Configuration,
    typeof(IdentityModule).Assembly,
    typeof(Modules.Events.Infrastructure.EventsModule).Assembly
);

var app = builder.Build();

// 6. HTTP Pipeline Yapılandırması
app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CS Mobil API v1");
        c.RoutePrefix = string.Empty; // Swagger doğrudan ana sayfada açılsın
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseAuthorization();

// 7. Modül Çalışma Zamanı Başlatıcıları
app.UseModules();

app.MapControllers();

app.Run();

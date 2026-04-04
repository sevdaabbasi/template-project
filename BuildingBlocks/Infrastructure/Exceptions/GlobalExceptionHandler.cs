using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Domain.Exceptions;

namespace BuildingBlocks.Infrastructure.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Hatayı logluyoruz
        _logger.LogError(exception, "Hata yakalandı: {Message}", exception.Message);

        // Hata tipine göre Durum Kodu
        var (statusCode, title) = exception switch
        {
            // Yanlış şifre veya e-posta (401)
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Yetkisiz Erişim"),
            
            // 5 kez yanlış deneme sonrası kilit (423)
            AccountLockedException => (StatusCodes.Status423Locked, "Hesap Kilitli"),
            
            // Kaynak bulunamadı (404)
            NotFoundException => (StatusCodes.Status404NotFound, "Bulunamadı"),
            
            // Zaten mevcut (409)
            AlreadyExistsException => (StatusCodes.Status409Conflict, "Zaten Mevcut"),
            
            // Genel çalışma hataları (400)
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Geçersiz İşlem"),
            
            // Diğer tüm bilinmeyen hatalar (500)
            _ => (StatusCodes.Status500InternalServerError, "Sunucu Hatası")
        };

        // ProblemDetails formatında (Standart HTTP Hatası) yanıt hazırlıyoruz
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message, 
            Instance = httpContext.Request.Path
        };

        // Yanıtı istemciye gönderiyoruz
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Geçersiz e-posta veya şifre.") { }
    }

    public class AccountLockedException : Exception
    {
        public AccountLockedException(DateTime? lockEndDate) 
            : base($"Çok fazla hatalı deneme. Hesabınız şu tarihe kadar kilitlendi: {lockEndDate?.ToLocalTime()}") { }
    }
}
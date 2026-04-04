using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace BuildingBlocks.Infrastructure.Logging;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
              // 1. İstekte varsa al, yoksa yeni bir GUID üret
        string correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var id)
            ? id!
            : Guid.NewGuid().ToString();

        // 2. Cevap header'ına ekle (Dış dünya görsün diye)
        context.Response.Headers.Append(CorrelationIdHeader, correlationId);

        // 3. LOG CONTEXT'e ekle (Serilog bunu otomatik yakalar!)
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}

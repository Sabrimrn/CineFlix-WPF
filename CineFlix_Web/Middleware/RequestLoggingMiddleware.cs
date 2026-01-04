using System.Diagnostics;

namespace CineFlix_Web.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // 1. Log dat er een request binnenkomt
            _logger.LogInformation($"[Middleware] 📥 Request ontvangen: {context.Request.Method} {context.Request.Path}");

            var stopwatch = Stopwatch.StartNew();

            // 2. Laat de rest van de applicatie zijn werk doen (de volgende middleware)
            await _next(context);

            stopwatch.Stop();

            // 3. Log hoe lang het duurde en wat de status is
            _logger.LogInformation($"[Middleware] ✅ Request afgehandeld in {stopwatch.ElapsedMilliseconds}ms - Status: {context.Response.StatusCode}");
        }
    }
}
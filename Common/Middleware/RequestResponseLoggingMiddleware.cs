using System.Diagnostics;

namespace GDB.Api.Common.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        public const string CorrelationIdHeader = "X-Correlation-Id";

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue)
                && !string.IsNullOrWhiteSpace(headerValue)
                    ? headerValue.ToString()
                    : context.TraceIdentifier;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeader] = correlationId;
                return Task.CompletedTask;
            });

            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                HttpRequest request = context.Request;

                _logger.LogInformation(
                    "HTTP {RequestMethod} {RequestPath}{QueryString} started from {RemoteIpAddress}",
                    request.Method,
                    request.Path,
                    request.QueryString,
                    context.Connection.RemoteIpAddress);

                long startTimestamp = Stopwatch.GetTimestamp();

                await _next(context);

                _logger.LogInformation(
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMilliseconds:0.0000} ms",
                    request.Method,
                    request.Path,
                    context.Response.StatusCode,
                    Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds);
            }
        }
    }
}

using GDB.Api.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GDB.Api.Common.ExceptionHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger)
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            (int statusCode, string title) = exception switch
            {
                InvalidAmountException => (StatusCodes.Status400BadRequest, "Invalid amount"),
                InvalidPinException => (StatusCodes.Status401Unauthorized, "Invalid PIN"),
                AccountException => (StatusCodes.Status422UnprocessableEntity, "Account operation failed"),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception");
            }
            else
            {
                _logger.LogWarning("{ExceptionType}: {ExceptionMessage}", exception.GetType().Name, exception.Message);
            }

            httpContext.Response.StatusCode = statusCode;

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = statusCode == StatusCodes.Status500InternalServerError ? null : exception.Message
                }
            });
        }
    }
}

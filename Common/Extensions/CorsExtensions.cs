using GDB.Api.Common.Middleware;

namespace GDB.Api.Common.Extensions
{
    public static class CorsExtensions
    {
        public const string DefaultPolicy = "DefaultCors";

        public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
        {
            string[] allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

            services.AddCors(options =>
            {
                options.AddPolicy(DefaultPolicy, policy => policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders(RequestResponseLoggingMiddleware.CorrelationIdHeader));
            });

            return services;
        }
    }
}

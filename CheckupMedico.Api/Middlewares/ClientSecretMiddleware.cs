namespace CheckupMedico.Api.Middlewares
{
    using CheckupMedico.Transversal.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public class ClientSecretMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _expectedSecret;
        private readonly ILogger<ClientSecretMiddleware> _logger;

        public ClientSecretMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ClientSecretMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _expectedSecret = configuration["Jwt:Key"] ?? "";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (context.Request.Method == HttpMethods.Options ||
                //path.StartsWith("/api/auth") ||
                path.StartsWith("/swagger") ||
                path.StartsWith("/openapi") ||
                path.StartsWith("/scalar") ||
                path.StartsWith("/favicon.ico"))
            {
                await _next(context);
                return;
            }

            // Verificar que exista el header
            if (!context.Request.Headers.TryGetValue("X-Client-Secret", out var providedSecret))
            {
                _logger.LogError($"Missing X-Client-Secret header for {path}");
                throw new UnauthorizedException("X-Client-Secret header es requerido.");
            }

            if (providedSecret != _expectedSecret)
            {
                _logger.LogError($"Invalid X-Client-Secret header for {path}");
                throw new UnauthorizedException("X-Client-Secret header incorrecto.");
            }

            await _next(context);
        }
    }
}

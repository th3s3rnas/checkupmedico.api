namespace CheckupMedico.Api.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsFromConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var origins = configuration
                .GetSection("CorsOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("AppCorsPolicy", policy =>
                {
                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return services;
        }
    }
}

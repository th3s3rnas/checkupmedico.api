using CheckupMedico.Api.Extensions;
using CheckupMedico.Api.Middlewares;
using CheckupMedico.Api.Models;
using CheckupMedico.Application.Doc;
using CheckupMedico.Application.Doc.Interface;
using CheckupMedico.Application.Service;
using CheckupMedico.Application.Service.Catalog;
using CheckupMedico.Application.Service.Checkup;
using CheckupMedico.Application.Service.Interface;
using CheckupMedico.Application.Service.Interface.Catalog;
using CheckupMedico.Application.Service.Interface.Checkup;
using CheckupMedico.Domain.Repository.Interface.LocalFile;
using CheckupMedico.Infrastructure.External.Apigateway.Colaborador;
using CheckupMedico.Infrastructure.External.Interface.Apigateway.Colaborador;
using CheckupMedico.Infrastructure.Repository.LocalFile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorsFromConfiguration(builder.Configuration);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // base
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


#region LOGGING
// Configurar Serilog desde appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});
#endregion

#region CONFIGURATION
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
#endregion

#region SERVICES
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
#endregion

#region JWT AUTHENTICATION
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();
#endregion

#region OPENAPI + SCALAR
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();
        document.Components.Parameters ??= new Dictionary<string, OpenApiParameter>();

        // Definición del header global
        document.Components.Parameters.Add("XClientSecret", new OpenApiParameter
        {
            Name = "X-Client-Secret",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Client secret required to consume the API",
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });

        // Agregar el header a todos los endpoints
        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                operation.Value.Parameters ??= new List<OpenApiParameter>();

                operation.Value.Parameters.Add(new OpenApiParameter
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Parameter,
                        Id = "XClientSecret"
                    }
                });
            }
        }

        return Task.CompletedTask;
    });
});
#endregion

#region DEPENDENCY INJECTION
builder.Services.AddHttpClient();
//Services
builder.Services.AddScoped<IColaboradorService, ColaboradorService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ICacheCleaner, CacheCleaner>();
builder.Services.AddScoped<ICheckupService, CheckupService>();
//Documents
builder.Services.AddScoped<ICheckupITESMDoc, CheckupITESMDoc>();
//Repositories
builder.Services.AddSingleton<IRepoLocalFileBillingConfig, RepoLocalFileBillingConfig>();
builder.Services.AddSingleton<IRepoLocalFileHospital, RepoLocalFileHospital>();
builder.Services.AddSingleton<IRepoLocalFileKit, RepoLocalFileKit>();
#endregion

var app = builder.Build();

app.UseRouting();
app.UseCors("AppCorsPolicy");

app.UseSerilogRequestLogging();

#region STARTUP CACHE CLEAN
using (var scope = app.Services.CreateScope())
{
    var cleaner = scope.ServiceProvider
        .GetRequiredService<ICacheCleaner>();

    cleaner.ClearAll();
}
#endregion

#region MIDDLEWARE PIPELINE
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ClientSecretMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
#endregion

app.Run();
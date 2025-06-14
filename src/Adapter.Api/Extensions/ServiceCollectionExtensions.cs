using Adapter.Api.ExceptionHandlers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace Adapter.Api.Extensions;
public static class ServiceCollectionExtensions
{
    public static void AddApiConfig(this IServiceCollection services, IConfiguration configuration)
    {
        string allowedOrigins = configuration.GetValue<string>("AllowedOrigins")!;
        services.AddCorsConfig(allowedOrigins);

        services.AddEndpointsApiExplorer();
        services.AddApiHealtChecksConfig();
        services.AddOpenApiConfig();
        services.AddCustomExceptionHandlerConfig();
        services.AddApiAuthorizationConfig();
        services.AddHttpContextAccessorConfig();
    }

    private static void AddHttpContextAccessorConfig(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
    }

    public static void AddOpenApiConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Farmacity Backend API", Version = "v1" });
        });
    }

    private static void AddCustomExceptionHandlerConfig(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    private static void AddApiHealtChecksConfig(this IServiceCollection services)
    {
        services.AddHealthChecks()
                        .AddCheck("Api Health", () => HealthCheckResult.Healthy("Api is healthy"));
    }

    private static void AddApiAuthorizationConfig(this IServiceCollection services)
    {
        services.AddAuthentication();
        services.AddAuthorization();
    }

    private static void AddCorsConfig(this IServiceCollection services, string allowedOrigins)
    {
        services.AddCors(opciones =>
        {

            opciones.AddDefaultPolicy(configuracion =>
            {
                configuracion.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
            });

            opciones.AddPolicy("open", configuracion =>
            {
                configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });
    }
}

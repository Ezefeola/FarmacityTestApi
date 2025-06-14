using Adapter.Api.Configurations.EndpointsConfig;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using System.Text.Json;

namespace Adapter.Api.Extensions;
public static class WebApplicationExtensions
{
    public static void AddApiWebApplicationConfig(this WebApplication app)
    {
        UseExtensionHandlerConfig(app);
        UseDevelopmentConfig(app);
        UseAuthorizationConfig(app);
        UseHealthChecksConfig(app);
        UseMapEndpointsConfig(app);
        UseHttpsRedirectionConfig(app);
    }

    private static void UseExtensionHandlerConfig(WebApplication app)
    {
        app.UseExceptionHandler();
    }

    private static void UseDevelopmentConfig(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/openapi/{documentName}.json";
            });
            app.MapScalarApiReference("/api/scalar", options =>
            {
                options.WithTitle("Farmacity Backend API")
                       .WithOpenApiRoutePattern("/api/openapi/v1.json")
                       .WithTheme(ScalarTheme.BluePlanet)
                       .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                       .AddHttpAuthentication("Bearer", options =>
                       {
                           options.Token = "text";
                       });
                options.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer"]
                };
            });
        }
    }

    private static void UseAuthorizationConfig(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }

    private static void UseHealthChecksConfig(WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    results = report.Entries.Select(entry => new
                    {
                        key = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        data = entry.Value.Data
                    })
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        });
    }

    private static void UseMapEndpointsConfig(WebApplication app)
    {
        app.MapEndpoints("/api");
    }

    private static void UseHttpsRedirectionConfig(WebApplication app)
    {
        app.UseHttpsRedirection();
    }
}

using Adapter.SqlServer.Data;
using Adapter.SqlServer.Repositories;
using Core.Contracts.Repositories;
using Core.Contracts.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Adapter.SqlServer;
public static class ServiceCollectionExtensions
{

    public static void AddAdapterSqlServer(this IServiceCollection services, string connectionString)
    {
        services.ConfigureDbContext(connectionString);
        services.ConfigureHealthChecks();
        services.AddUnitOfWork();
        services.AddRepositories();
    }

    private static void ConfigureDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }

    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("Database Health", HealthStatus.Unhealthy);
    }

    private static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductoRepository, ProductoRepository>()
                .AddScoped<ICodigoBarraRepository, CodigoBarraRepository>();
    }
}
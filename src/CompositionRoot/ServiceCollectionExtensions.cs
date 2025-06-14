using Adapter.SqlServer;
using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot;
public static class ServiceCollectionExtensions
{
    public static void AddCompositionRoot(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("FarmacityTestDb")
        ?? throw new KeyNotFoundException("Error: Connection string not found.");

        services.AddCore();
        services.AddAdapterSqlServer(connectionString);
    }
}
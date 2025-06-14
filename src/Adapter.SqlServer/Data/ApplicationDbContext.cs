using Adapter.SqlServer.Data.Converters;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Adapter.SqlServer.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<DateTime>()
        .HaveConversion<ValueConverters.UtcValueConverter>();

        configurationBuilder.Properties<DateTime?>()
            .HaveConversion<ValueConverters.NullableUtcValueConverter>();
    }
}
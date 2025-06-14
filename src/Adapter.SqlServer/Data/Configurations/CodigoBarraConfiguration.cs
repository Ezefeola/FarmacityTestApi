using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adapter.SqlServer.Data.Configurations;
public class CodigoBarraConfiguration : EntityTypeBaseConfiguration<CodigoBarra>
{
    protected override void ConfigurateConstraints(EntityTypeBuilder<CodigoBarra> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Producto)
               .WithMany(x => x.CodigosBarras)
               .HasForeignKey(x => x.ProductoId);
    }

    protected override void ConfigurateProperties(EntityTypeBuilder<CodigoBarra> builder)
    {
        builder.Property(x => x.Codigo)
               .HasMaxLength(CodigoBarra.Rules.CODIGO_MAX_LENGTH);
        builder.HasIndex(x => x.Codigo)
               .IsUnique();

        BaseEntityConfig.ApplyTo<CodigoBarra, int>(builder);
    }
}
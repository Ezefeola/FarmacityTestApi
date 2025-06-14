using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adapter.SqlServer.Data.Configurations;
public class ProductoConfiguration : EntityTypeBaseConfiguration<Producto>
{
    protected override void ConfigurateConstraints(EntityTypeBuilder<Producto> builder)
    {
        builder.HasKey(x => x.Id);
    }

    protected override void ConfigurateProperties(EntityTypeBuilder<Producto> builder)
    {
        builder.Property(x => x.Nombre)
                .HasMaxLength(Producto.Rules.NOMBRE_MAX_LENGTH);
        builder.HasIndex(p => p.Nombre);

        builder.Property(x => x.Precio)
               .HasPrecision(18, 2);

        builder.Property(x => x.CantidadEnStock);

        BaseEntityConfig.ApplyTo<Producto, int>(builder);
    }
}
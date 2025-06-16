using Core.Domain.Abstractions;

namespace Core.Domain.Entities;
public class CodigoBarra : Entity<int>
{
    public CodigoBarra() { }
    public CodigoBarra(
        int id,
        string codigo,
        bool activo,
        DateTime fechaAlta,
        DateTime? fechaModificacion = null
    )
    {
        Id = id;
        Codigo = codigo;
        Activo = activo;
        FechaAlta = fechaAlta;
        FechaModificacion = fechaModificacion;
    }

    public static class Rules
    {
        public const int CODIGO_MAX_LENGTH = 255;
    }

    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = default!;
    public string Codigo { get; set; } = default!;

    public void UpdateIfChanged(string newCodigoBarra)
    {
        if (!Codigo.Equals(newCodigoBarra, StringComparison.OrdinalIgnoreCase))
        {
            Codigo = newCodigoBarra;
            Activo = true;
            FechaModificacion = DateTime.UtcNow;
        }

        if (!Activo)
        {
            Activo = true;
            FechaModificacion = DateTime.UtcNow;
        }
    }

    public CodigoBarra SoftDelete()
    {
        if (!Activo) return this;

        Activo = false;
        FechaModificacion = DateTime.UtcNow;

        return this;
    }
}
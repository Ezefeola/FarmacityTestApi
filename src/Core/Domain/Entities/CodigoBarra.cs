using Core.Domain.Abstractions;
using static Core.Utilities.Validations.ValidationMessages;

namespace Core.Domain.Entities;
public class CodigoBarra : Entity<int>
{
    public static class Rules
    {
        public const int CODIGO_MAX_LENGTH = 255;
    }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = default!;
    public string Codigo { get; set; } = default!;

    public CodigoBarra SoftDelete()
    {
        if (!Activo) return this;

        Activo = false;
        FechaModificacion = DateTime.UtcNow;

        return this;
    }
}
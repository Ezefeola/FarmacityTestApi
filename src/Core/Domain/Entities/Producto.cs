using Core.Domain.Abstractions;

namespace Core.Domain.Entities;
public class Producto : Entity<int>
{
    public static class Rules
    {
        public const int NOMBRE_MAX_LENGTH = 255;
    }

    public string Nombre { get; set; } = default!;
    public decimal Precio { get; set; }
    public int CantidadEnStock { get; set; }
    public List<CodigoBarra> CodigosBarras { get; set; } = [];
}
using Core.Domain.Abstractions;

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
}
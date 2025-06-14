using Core.Contracts.DTOs.CodigosBarras.Response;

namespace Core.Contracts.DTOs.Productos.Response;
public sealed record SoftDeleteProductoResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public bool Activo { get; set; }
    public List<CodigoBarraResponseDto>? CodigosBarras { get; set; }
}
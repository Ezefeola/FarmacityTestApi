using Core.Contracts.DTOs.CodigosBarras.Response;

namespace Core.Contracts.DTOs.Productos.Response;
public sealed record GetProductoByIdResponseDto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = default!;
    public decimal Precio { get; set; }
    public int CantidadEnStock { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public List<CodigoBarraResponseDto> CodigosBarras { get; set; } = [];
}
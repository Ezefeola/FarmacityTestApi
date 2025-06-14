using Core.Contracts.DTOs.CodigosBarras.Request;

namespace Core.Contracts.DTOs.Productos.Request;
public sealed record UpdateProductoRequestDto
{
    public string? Nombre { get; set; }
    public decimal? Precio { get; set; }
    public int? CantidadEnStock { get; set; }
    public List<UpdateCodigoBarraRequestDto> CodigosBarras { get; set; } = [];
}
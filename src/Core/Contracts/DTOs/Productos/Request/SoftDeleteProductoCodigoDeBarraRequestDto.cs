namespace Core.Contracts.DTOs.Productos.Request;
public sealed record SoftDeleteProductoCodigoDeBarraRequestDto
{
    public List<int> CodigoBarraIds { get; set; } = [];
}
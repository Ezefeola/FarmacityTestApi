namespace Core.Contracts.DTOs.Productos.Request;
public sealed record SoftDeleteProductoCodigoDeBarraRequestDto
{
    public int[] CodigoBarraIds { get; set; } = [];
}
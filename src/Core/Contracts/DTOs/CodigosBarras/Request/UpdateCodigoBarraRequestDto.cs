namespace Core.Contracts.DTOs.CodigosBarras.Request;
public sealed record UpdateCodigoBarraRequestDto
{
    public int? CodigoBarraId { get; set; }
    public required string Codigo { get; set; }
}
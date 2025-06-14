namespace Core.Contracts.DTOs.CodigosBarras.Request;
public sealed record UpdateCodigoBarraRequestDto
{
    public required string Codigo { get; set; }
}
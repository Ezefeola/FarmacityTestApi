namespace Core.Contracts.DTOs.CodigosBarras.Request;
public sealed record CreateCodigoBarraRequestDto
{
    public required string Codigo { get; set; }
}
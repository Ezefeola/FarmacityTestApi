namespace Core.Contracts.DTOs.CodigosBarras.Response;
public sealed record CodigoBarraResponseDto
{
    public int CodigoBarraId { get; set; }
    public string Codigo { get; set; } = default!;
    public bool Activo { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaModificacion { get; set; }
}
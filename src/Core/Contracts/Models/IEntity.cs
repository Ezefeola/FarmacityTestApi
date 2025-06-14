namespace Core.Contracts.Models;
public interface IEntity<TId>
{
    public TId Id { get; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public bool Activo { get; set; }
}
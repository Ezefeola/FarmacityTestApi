using Core.Contracts.DTOs.Productos.Request;
using Core.Domain.Abstractions;

namespace Core.Domain.Entities;
public class Producto : Entity<int>
{
    public static class Rules
    {
        public const int NOMBRE_MAX_LENGTH = 255;
    }

    public string Nombre { get; set; } = default!;
    public decimal Precio { get; set; }
    public int CantidadEnStock { get; set; }
    public List<CodigoBarra> CodigosBarras { get; set; } = [];

    public Producto SoftDelete()
    {
        if (!Activo) return this;

        Activo = false;
        FechaModificacion = DateTime.UtcNow;
        if(CodigosBarras.Count > 0)
        {
            foreach (CodigoBarra codigoBarra in CodigosBarras)
            {
                codigoBarra.Activo = false;
                codigoBarra.FechaModificacion = DateTime.UtcNow;
            }
        }

        return this;
    }

    public void UpdateIfChanged(UpdateProductoRequestDto requestDto, List<string> codigosBarras)
    {
        bool anyUpdated = false;
        if (!string.IsNullOrWhiteSpace(requestDto.Nombre))
        {
            Nombre = requestDto.Nombre;
            anyUpdated = true;
        }

        if (requestDto.Precio.HasValue)
        {
            Precio = requestDto.Precio.Value;
            anyUpdated = true;
        }
         
        if (requestDto.CantidadEnStock.HasValue)
        {
            CantidadEnStock = requestDto.CantidadEnStock.Value;
            anyUpdated = true;
        }

        IEnumerable<string> associatedCodigosBarras = CodigosBarras.Select(x => x.Codigo);
        IEnumerable<CodigoBarra> newCodigosBarras = codigosBarras
                                                        .Except(associatedCodigosBarras, StringComparer.OrdinalIgnoreCase)
                                                        .Select(codigo => new CodigoBarra
                                                        {
                                                            Codigo = codigo,
                                                            Activo = true,
                                                            FechaAlta = DateTime.UtcNow
                                                        });
        if (newCodigosBarras.Count() > 0)
        {
            CodigosBarras.AddRange(newCodigosBarras);
            anyUpdated = true;
        }


        if (anyUpdated)
        {
            FechaModificacion = DateTime.UtcNow;
        }
    }
}
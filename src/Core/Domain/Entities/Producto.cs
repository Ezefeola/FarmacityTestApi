using Core.Domain.Abstractions;

namespace Core.Domain.Entities;
public class Producto : Entity<int>
{
    public Producto() { }
    public Producto(
        int id, 
        string nombre, 
        decimal precio, 
        int cantidadEnStock, 
        bool activo, 
        DateTime? fechaAlta = null
    )
    {
        Id = id;
        Nombre = nombre;
        Precio = precio;
        CantidadEnStock = cantidadEnStock;
        Activo = activo;
        FechaAlta = fechaAlta ?? DateTime.UtcNow;
        CodigosBarras = [];
    }
    public static class Rules
    {
        public const int NOMBRE_MAX_LENGTH = 255;
    }

    public string Nombre { get; set; } = default!;
    public decimal Precio { get; set; }
    public int CantidadEnStock { get; set; }
    public List<CodigoBarra> CodigosBarras { get; set; } = [];

    public void UpdateIfChanged(
        string? nombre,
        decimal? precio,
        int? cantidadEnStock
    )
    {
        bool anyUpdated = false;

        if (!string.IsNullOrWhiteSpace(nombre) && !nombre.Equals(Nombre, StringComparison.OrdinalIgnoreCase))
        {
            Nombre = nombre;
            anyUpdated = true;
        }

        if (precio.HasValue && precio.Value != Precio)
        {
            Precio = precio.Value;
            anyUpdated = true;
        }

        if (cantidadEnStock.HasValue && cantidadEnStock.Value != CantidadEnStock)
        {
            CantidadEnStock = cantidadEnStock.Value;
            anyUpdated = true;
        }

        if (anyUpdated)
        {
            FechaModificacion = DateTime.UtcNow;
        }
    }

    public void AddNewCodigosBarras(IEnumerable<string> newCodigosBarras)
    {
        HashSet<string> existingCodigosBarras = CodigosBarras
                                                        .Select(x => x.Codigo)
                                                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (string codigoBarra in newCodigosBarras)
        {
            if (!existingCodigosBarras.Contains(codigoBarra))
            {
                CodigosBarras.Add(new CodigoBarra
                {
                    Codigo = codigoBarra,
                    Activo = true,
                    FechaAlta = DateTime.UtcNow
                });
            }
        }
    }

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
}
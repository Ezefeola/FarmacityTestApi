using Core.Contracts.DTOs.Productos.Request;
using Core.Domain.Entities;

namespace Core.Utilities.QueryOptions.QueryableExtensions;
public static class ProductoQueryableExtensions
{
    public static IQueryable<Producto> ApplyFilteringToGetProductos(
        this IQueryable<Producto> getProductosQuery,
        GetProductosRequestDto filters)
    {
        if (!string.IsNullOrWhiteSpace(filters.Nombre))
        {
            getProductosQuery = getProductosQuery.Where(p => p.Nombre.Contains(filters.Nombre));
        }

        if (filters.PrecioMin.HasValue)
        {
            getProductosQuery = getProductosQuery.Where(p => p.Precio >= filters.PrecioMin.Value);
        }

        if (filters.PrecioMax.HasValue)
        {
            getProductosQuery = getProductosQuery.Where(p => p.Precio <= filters.PrecioMax.Value);
        }

        return getProductosQuery;
    }
}
using Core.Utilities.QueryOptions.Pagination;
using Core.Utilities.QueryOptions.Sorting;

namespace Core.Contracts.DTOs.Productos.Request;
public sealed record GetProductosRequestDto : IHasPaginationOptions, IHasSortingOptions
{
    public int? Page { get; set; }

    public int? PageSize { get; set; }

    public string? SortColumn { get; set; }
    public bool? SortDescending { get; set; } = false;

    public string? Nombre { get; set; }
    public decimal? PrecioMin { get; set; }
    public decimal? PrecioMax { get; set; }
}
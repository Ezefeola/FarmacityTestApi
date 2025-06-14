using Core.Utilities.QueryOptions.Pagination;

namespace Core.Contracts.DTOs.Productos.Request;
public sealed record GetProductosRequestDto : IHasPaginationOptions
{
    public int? Page { get; set; }

    public int? PageSize { get; set; }
}
namespace Core.Utilities.QueryOptions.Pagination;
public interface IHasPaginationOptions
{
    int? Page { get; set; }
    int? PageSize { get; set; }
}
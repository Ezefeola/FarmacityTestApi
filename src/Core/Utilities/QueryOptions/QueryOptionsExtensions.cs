using Core.Utilities.QueryOptions.Pagination;

namespace Core.Utilities.QueryOptions;
public static class QueryOptionsExtensions
{
    public static int GetPage(this IHasPaginationOptions options)
    {
        return options.Page is > 0 ? options.Page.Value : 1;
    }

    public static int GetPageSize(this IHasPaginationOptions options)
    {
        return options.PageSize is > 0 ? options.PageSize.Value : 10;
    }

    public static int GetTotalPages(this IHasPaginationOptions options, int count)
    {
        if (count <= 0) return 1;
        int totalPages = count / options.GetPageSize();
        if (count % options.GetPageSize() != 0) totalPages++;
        return totalPages;
    }
}
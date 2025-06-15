namespace Core.Utilities.QueryOptions.QueryableExtensions;
public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> queriable,
        int page,
        int pageSize
    )
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        return queriable.Skip((page - 1) * pageSize)
                        .Take(pageSize);
    }
}

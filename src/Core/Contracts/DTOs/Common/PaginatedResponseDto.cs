namespace Core.Contracts.DTOs.Common;
public record PaginatedResponseDto<TItems>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public IEnumerable<TItems> Items { get; set; } = [];
}
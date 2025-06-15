namespace Core.Utilities.QueryOptions.Sorting;
public interface IHasSortingOptions
{
    public string? SortColumn { get; set; }
    public bool? SortDescending { get; set; }
}

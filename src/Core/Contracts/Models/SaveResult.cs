namespace Core.Contracts.Models;
public class SaveResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int RowsAffected { get; set; }
}
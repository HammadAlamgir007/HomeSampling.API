namespace Shared.Infrastructure.Models;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PerPage);
}

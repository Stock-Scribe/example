namespace Nexon.FleaMarket.Domain.VO;

public class ProductSearchQuery
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public long? MinPrice { get; set; }
    public long? MaxPrice { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
}
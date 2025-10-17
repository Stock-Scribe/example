namespace Nexon.FleaMarket.Application.Dto.request;

public class ProductSearchRequest
{
    public string? SearchKeyword { get; set; }
    public int? CategoryId { get; set; }
    public long? MinPrice { get; set; }
    public long? MaxPrice { get; set; }
    public string SortBy { get; set; } = "price";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
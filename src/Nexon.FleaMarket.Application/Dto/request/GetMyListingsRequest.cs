namespace Nexon.FleaMarket.Application.Dto.request;

public class GetMyListingsRequest
{
    public long UserId { get; set; }
    public string Type { get; set; } = "ALL"; // ALL, SELLING, BUYING, CANCELLED
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
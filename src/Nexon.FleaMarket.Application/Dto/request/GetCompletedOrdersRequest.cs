namespace Nexon.FleaMarket.Application.Dto.request;

public class GetCompletedOrdersRequest
{
    public long UserId { get; set; }
    public string Type { get; set; } = "ALL"; // ALL, BUY, SELL
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
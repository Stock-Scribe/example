namespace Nexon.FleaMarket.Application.Dto.response;

public class CompletedOrderDto
{
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public long TotalPrice { get; set; }
    public string TradeType { get; set; } = string.Empty; // BUY(구매), SELL(판매)
    public string CounterPartyName { get; set; } = string.Empty; // 거래 상대방
    public DateTime CreatedAt { get; set; }
}
namespace Nexon.FleaMarket.Application.Dto.response;

public class CreateBuyListingResponse
{
    public int ResultCode { get; set; }
    public string ResultMessage { get; set; } = string.Empty;
    public bool IsMatched { get; set; }
    public long? OrderId { get; set; }
    public long? BuyListingId { get; set; }
    public int ProcessedQuantity { get; set; }
    public int RemainingQuantity { get; set; }
}
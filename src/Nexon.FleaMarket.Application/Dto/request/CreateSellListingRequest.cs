namespace Nexon.FleaMarket.Application.Dto.request;

public class CreateSellListingRequest
{
    public long ProductId { get; set; }
    public long SellerId { get; set; }
    public long ItemPrice {get; set;}
    public int Quantity {get; set;}
}
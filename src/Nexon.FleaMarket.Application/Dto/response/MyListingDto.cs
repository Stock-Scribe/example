namespace Nexon.FleaMarket.Application.Dto.response;

public class MyListingDto
{
    public long ListingId { get; set; }
    public string ListingType { get; set; } = string.Empty; // SELL, BUY
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public long ItemPrice { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty; // ACTIVE, SOLD_OUT, FILLED, CANCELLED
    public DateTime CreatedAt { get; set; }
}
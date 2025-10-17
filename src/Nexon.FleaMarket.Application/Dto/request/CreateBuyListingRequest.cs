namespace Nexon.FleaMarket.Application.Dto.request;

public class CreateBuyListingRequest
{
    public long ProductId { get; init; }      // 어떤 상품을 살 건지
    public long BuyerId { get; init; }        // 누가 사는지
    public long BidPrice { get; init; }       // 희망 구매가 (BidPriceSP)
    public int Quantity { get; init; }        // 수량
}
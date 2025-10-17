namespace Nexon.FleaMarket.Application.Dto.response;

public class CreateBuyListingResponse
{
    public long BuyListingId { get; init; }   // 생성된 구매 리스팅 ID
    public long? OrderId { get; init; }       // 매칭된 경우 생성된 주문 ID (없으면 null)
}
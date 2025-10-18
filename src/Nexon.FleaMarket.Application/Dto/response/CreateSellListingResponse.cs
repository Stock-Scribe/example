namespace Nexon.FleaMarket.Application.Dto.response;

public class CreateSellListingResponse
{
    /// <summary>
    /// 즉시 거래 성사 여부
    /// </summary>
    public bool IsMatched { get; set; }
    
    /// <summary>
    /// 거래 성사 시 OrderId, 아니면 null
    /// </summary>
    public long? OrderId { get; set; }
    
    /// <summary>
    /// 판매 등록 시 ListingId, 거래 성사 시 null
    /// </summary>
    public long? ListingId { get; set; }
    
    /// <summary>
    /// 거래/등록된 수량
    /// </summary>
    public int ProcessedQuantity { get; set; }
    
    /// <summary>
    /// 대기 중인 수량 (부분 매칭 시)
    /// </summary>
    public int RemainingQuantity { get; set; }
}
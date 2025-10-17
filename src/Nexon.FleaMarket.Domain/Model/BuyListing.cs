namespace Nexon.FleaMarket.Domain.Entities;

/// <summary>
/// 구매 등록(입찰) 엔티티
/// </summary>
public class BuyListing
{
    public long Id { get; private set; }
    public long ProductId { get; private set; }
    public long BuyerId { get; private set; }
    public long BidPrice { get; private set; }
    public int Quantity { get; private set; }
    public string Status { get; private set; } // ACTIVE, FILLED, CANCELLED
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected BuyListing() { }

    public BuyListing(long productId, long buyerId, long bidPrice, int quantity)
    {
        ProductId = productId;
        BuyerId = buyerId;
        BidPrice = bidPrice;
        Quantity = quantity;
        Status = "ACTIVE";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 활성 상태인지 확인
    /// </summary>
    public bool IsActive() => Status == "ACTIVE";

    /// <summary>
    /// 남은 수량 확인
    /// </summary>
    public bool HasRemainingQuantity(int requestedQuantity)
    {
        return IsActive() && Quantity >= requestedQuantity;
    }

    /// <summary>
    /// 총 가격 계산
    /// </summary>
    public long CalculateTotalPrice(int quantity)
    {
        return BidPrice * quantity;
    }

    /// <summary>
    /// 구매 수량 차감
    /// </summary>
    public void ReduceQuantity(int filledQuantity)
    {
        if (!HasRemainingQuantity(filledQuantity))
            throw new InvalidOperationException($"수량 부족: 남은 {Quantity}개, 요청 {filledQuantity}개");

        Quantity -= filledQuantity;
        UpdatedAt = DateTime.UtcNow;

        if (Quantity == 0)
            MarkAsFilled();
    }

    /// <summary>
    /// 체결 완료 처리
    /// </summary>
    public void MarkAsFilled()
    {
        Status = "FILLED";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 입찰 취소
    /// </summary>
    public void Cancel()
    {
        if (Status == "FILLED")
            throw new InvalidOperationException("이미 체결된 입찰입니다");

        Status = "CANCELLED";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 입찰가 변경
    /// </summary>
    public void UpdateBidPrice(long newBidPrice)
    {
        if (!IsActive())
            throw new InvalidOperationException("활성 상태가 아닌 입찰은 가격을 변경할 수 없습니다");

        BidPrice = newBidPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}
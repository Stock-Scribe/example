namespace Nexon.FleaMarket.Domain.Entities;

/// <summary>
/// 판매 등록 엔티티
/// </summary>
public class SellListing
{
    public long Id { get; private set; }
    public long ProductId { get; private set; }
    public long SellerId { get; private set; }
    public long UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public string Status { get; private set; } // ACTIVE, SOLD_OUT, CLOSED
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected SellListing() { }

    public SellListing(long productId, long sellerId, long unitPrice, int quantity)
    {
        ProductId = productId;
        SellerId = sellerId;
        UnitPrice = unitPrice;
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
    /// 재고가 충분한지 확인
    /// </summary>
    public bool HasEnoughStock(int requestedQuantity)
    {
        return IsActive() && Quantity >= requestedQuantity;
    }

    /// <summary>
    /// 총 가격 계산
    /// </summary>
    public long CalculateTotalPrice(int quantity)
    {
        return UnitPrice * quantity;
    }

    /// <summary>
    /// 판매 수량 차감
    /// </summary>
    public void ReduceStock(int soldQuantity)
    {
        if (!HasEnoughStock(soldQuantity))
            throw new InvalidOperationException($"재고 부족: 보유 {Quantity}개, 요청 {soldQuantity}개");

        Quantity -= soldQuantity;
        UpdatedAt = DateTime.UtcNow;

        if (Quantity == 0)
            MarkAsSoldOut();
    }

    /// <summary>
    /// 품절 처리
    /// </summary>
    public void MarkAsSoldOut()
    {
        Status = "SOLD_OUT";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 판매 취소
    /// </summary>
    public void Close()
    {
        if (Status == "SOLD_OUT")
            throw new InvalidOperationException("이미 품절된 판매입니다");

        Status = "CLOSED";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 가격 변경
    /// </summary>
    public void UpdatePrice(long newPrice)
    {
        if (!IsActive())
            throw new InvalidOperationException("활성 상태가 아닌 판매는 가격을 변경할 수 없습니다");

        UnitPrice = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}

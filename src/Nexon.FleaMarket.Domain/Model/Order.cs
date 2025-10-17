namespace Nexon.FleaMarket.Domain.Entities;

/// <summary>
/// 주문(거래 체결) 엔티티
/// </summary>
public class Order
{
    public long Id { get; private set; }
    public long? SellListingId { get; private set; }
    public long? BuyListingId { get; private set; }
    public long BuyerId { get; private set; }
    public long SellerId { get; private set; }
    public long ProductId { get; private set; }
    public int Quantity { get; private set; }
    public long TotalPrice { get; private set; }
    public string Status { get; private set; } // PENDING, PAID, FULFILLED, CANCELLED
    public string IdempotencyKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected Order() { }

    public Order(
        long? sellListingId,
        long? buyListingId,
        long buyerId,
        long sellerId,
        long productId,
        int quantity,
        long totalPrice,
        string idempotencyKey)
    {
        SellListingId = sellListingId;
        BuyListingId = buyListingId;
        BuyerId = buyerId;
        SellerId = sellerId;
        ProductId = productId;
        Quantity = quantity;
        TotalPrice = totalPrice;
        Status = "PENDING";
        IdempotencyKey = idempotencyKey;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 즉시 구매 주문 생성
    /// </summary>
    public static Order CreateInstantBuyOrder(
        long sellListingId,
        long buyerId,
        long sellerId,
        long productId,
        int quantity,
        long totalPrice,
        string idempotencyKey)
    {
        return new Order(sellListingId, null, buyerId, sellerId, productId, quantity, totalPrice, idempotencyKey);
    }

    /// <summary>
    /// 매칭된 거래 주문 생성
    /// </summary>
    public static Order CreateMatchedOrder(
        long sellListingId,
        long buyListingId,
        long buyerId,
        long sellerId,
        long productId,
        int quantity,
        long totalPrice,
        string idempotencyKey)
    {
        return new Order(sellListingId, buyListingId, buyerId, sellerId, productId, quantity, totalPrice, idempotencyKey);
    }

    /// <summary>
    /// 즉시 구매 주문인지 확인
    /// </summary>
    public bool IsInstantBuy() => SellListingId.HasValue && !BuyListingId.HasValue;

    /// <summary>
    /// 매칭 거래인지 확인
    /// </summary>
    public bool IsMatchedTrade() => SellListingId.HasValue && BuyListingId.HasValue;

    /// <summary>
    /// 결제 완료 처리
    /// </summary>
    public void MarkAsPaid()
    {
        if (Status != "PENDING")
            throw new InvalidOperationException($"PENDING 상태만 결제 가능합니다 (현재: {Status})");

        Status = "PAID";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 이행 완료 처리 (아이템 전달 완료)
    /// </summary>
    public void Complete()
    {
        if (Status != "PAID")
            throw new InvalidOperationException($"PAID 상태만 완료 가능합니다 (현재: {Status})");

        Status = "FULFILLED";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 주문 취소
    /// </summary>
    public void Cancel()
    {
        if (Status == "FULFILLED")
            throw new InvalidOperationException("이미 완료된 주문은 취소할 수 없습니다");

        Status = "CANCELLED";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 주문 상태 확인
    /// </summary>
    public bool IsPending() => Status == "PENDING";
    public bool IsPaid() => Status == "PAID";
    public bool IsCompleted() => Status == "FULFILLED";
    public bool IsCancelled() => Status == "CANCELLED";
}

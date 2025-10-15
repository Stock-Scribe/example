namespace Nexon.FleaMarket.Domain.Entities;

public class Listing
{
    public long ListingId { get; private set; }      // PK
    public long ProductId { get; private set; }      // FK → Product
    public long SellerId { get; private set; }       // FK → User
    public long UnitPrice { get; private set; }      // 개당 판매가 (DB에선 UnitPriceSP로 매핑 예정)
    public int Quantity { get; private set; }        // 남은 수량
    public string Status { get; private set; } = null!; // "ACTIVE" | "SOLD_OUT" | "CLOSED"
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Listing() { } // EF Core용

    public static Listing Create(long productId, long sellerId, long unitPrice, int quantity)
    {
        if (productId <= 0) throw new ArgumentException("Invalid productId.");
        if (sellerId <= 0)  throw new ArgumentException("Invalid sellerId.");
        if (unitPrice <= 0) throw new ArgumentException("UnitPrice must be greater than 0.");
        if (quantity  <= 0) throw new ArgumentException("Quantity must be greater than 0.");

        return new Listing
        {
            ProductId = productId,
            SellerId  = sellerId,
            UnitPrice = unitPrice,
            Quantity  = quantity,
            Status    = "ACTIVE",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // 수량 예약(구매 시 차감). SOLD_OUT 처리까지 포함
    public void Reserve(int qty)
    {
        if (!IsActive()) throw new InvalidOperationException("Listing is not ACTIVE.");
        if (qty <= 0)    throw new ArgumentException("qty must be > 0.");
        if (qty > Quantity) throw new InvalidOperationException("Not enough quantity.");

        Quantity -= qty;
        if (Quantity == 0) Status = "SOLD_OUT";
        UpdatedAt = DateTime.UtcNow;
    }

    // 예약 취소/재고 복구 (주문 취소 등)
    public void Restore(int qty)
    {
        if (qty <= 0) throw new ArgumentException("qty must be > 0.");
        Quantity += qty;
        if (Status == "SOLD_OUT" && Quantity > 0) Status = "ACTIVE";
        UpdatedAt = DateTime.UtcNow;
    }

    // 가격 변경
    public void ChangePrice(long newUnitPrice)
    {
        if (newUnitPrice <= 0) throw new ArgumentException("UnitPrice must be > 0.");
        if (newUnitPrice == UnitPrice) return;
        UnitPrice = newUnitPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    // 수동 재고 추가(셀러가 물량 추가)
    public void AddStock(int qty)
    {
        if (qty <= 0) throw new ArgumentException("qty must be > 0.");
        Quantity += qty;
        if (Status != "CLOSED") Status = "ACTIVE";
        UpdatedAt = DateTime.UtcNow;
    }

    // 판매 종료(노출 중지). 재개 가능하도록 별도 메서드 둠
    public void Close()
    {
        Status = "CLOSED";
        UpdatedAt = DateTime.UtcNow;
    }

    // 재개(재고>0이어야 의미 있음)
    public void Reopen()
    {
        if (Quantity <= 0) throw new InvalidOperationException("No stock to reopen.");
        Status = "ACTIVE";
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive() => Status == "ACTIVE";
}

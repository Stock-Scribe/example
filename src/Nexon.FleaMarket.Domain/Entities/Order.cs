namespace Nexon.FleaMarket.Domain.Entities;

public class Order
{
    public long OrderId { get; private set; }        // PK
    public long ListingId { get; private set; }      // FK → Listing
    public long BuyerId { get; private set; }        // FK → User
    public int Quantity { get; private set; }
    public long UnitPrice { get; private set; }      // 스냅샷 가격(주문 시점)
    public long TotalPrice { get; private set; }     // UnitPrice * Quantity
    public string Status { get; private set; } = null!; // "PENDING" | "PAID" | "FULFILLED" | "CANCELLED"
    public string? IdempotencyKey { get; private set; }  // 중복 방지 키(유니크)
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Order() { }

    public static Order Create(long listingId, long buyerId, int quantity, long unitPrice, string? idempotencyKey = null)
    {
        if (listingId <= 0) throw new ArgumentException("Invalid listingId.");
        if (buyerId  <= 0) throw new ArgumentException("Invalid buyerId.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be > 0.");
        if (unitPrice <= 0) throw new ArgumentException("UnitPrice must be > 0.");

        return new Order
        {
            ListingId = listingId,
            BuyerId = buyerId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = checked(unitPrice * quantity),
            Status = "PENDING",
            IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? null : idempotencyKey.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkPaid()
    {
        if (Status != "PENDING") throw new InvalidOperationException("Order must be PENDING to mark PAID.");
        Status = "PAID";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fulfill()
    {
        if (Status != "PAID") throw new InvalidOperationException("Order must be PAID to fulfill.");
        Status = "FULFILLED";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == "FULFILLED") throw new InvalidOperationException("Cannot cancel a fulfilled order.");
        if (Status == "CANCELLED") return; // idempotent
        Status = "CANCELLED";
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsTerminal() => Status is "FULFILLED" or "CANCELLED";
}
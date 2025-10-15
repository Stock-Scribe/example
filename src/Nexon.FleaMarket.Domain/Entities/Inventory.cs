namespace Nexon.FleaMarket.Domain.Entities;

public class Inventory
{
    public long InventoryId { get; private set; } // PK
    public long UserId { get; private set; }      // FK → User
    public long ProductId { get; private set; }   // FK → Product
    public long SourceOrderId { get; private set; } // FK → Order
    public int Quantity { get; private set; }
    public DateTime? ExpireAt { get; private set; }  // 기간제만 값 존재
    public DateTime AcquiredAt { get; private set; }

    private Inventory() { }

    public static Inventory Create(long userId, long productId, long sourceOrderId, int quantity, DateTime? expireAt = null)
    {
        if (userId <= 0) throw new ArgumentException("Invalid userId.");
        if (productId <= 0) throw new ArgumentException("Invalid productId.");
        if (sourceOrderId <= 0) throw new ArgumentException("Invalid sourceOrderId.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be > 0.");

        return new Inventory
        {
            UserId = userId,
            ProductId = productId,
            SourceOrderId = sourceOrderId,
            Quantity = quantity,
            ExpireAt = expireAt,
            AcquiredAt = DateTime.UtcNow
        };
    }

    public void AddQuantity(int qty)
    {
        if (qty <= 0) throw new ArgumentException("qty must be > 0.");
        Quantity += qty;
    }

    public bool IsExpired(DateTime utcNow) => ExpireAt.HasValue && utcNow >= ExpireAt.Value;
}
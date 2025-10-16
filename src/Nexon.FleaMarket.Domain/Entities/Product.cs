namespace Nexon.FleaMarket.Domain.Entities;

public class Product
{
    public long Id { get; private set; }
    public int ExternalItemNo { get; private set; }
    public string Name { get; private set; }
    public int CategoryId { get; private set; }
    public long BasePrice { get; private set; }
    public string DurationType { get; private set; } // PERMANENT / TEMPORARY
    public int? DurationDays { get; private set; }
    public string ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected Product() { }

    public Product(
        int externalItemNo,
        string name,
        int categoryId,
        long basePrice,
        string durationType,
        int? durationDays = null,
        string imageUrl = null)
    {
        ExternalItemNo = externalItemNo;
        Name = name;
        CategoryId = categoryId;
        BasePrice = basePrice;
        DurationType = durationType;
        DurationDays = durationDays;
        ImageUrl = imageUrl;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 영구 아이템인지 확인
    /// </summary>
    public bool IsPermanent() => DurationType == "PERMANENT";

    /// <summary>
    /// 기간제 아이템인지 확인
    /// </summary>
    public bool IsTemporary() => DurationType == "TEMPORARY";

    /// <summary>
    /// 만료일 계산
    /// </summary>
    public DateTime? CalculateExpirationDate()
    {
        if (IsPermanent()) return null;
        return DateTime.UtcNow.AddDays(DurationDays ?? 0);
    }

    /// <summary>
    /// 가격 변경
    /// </summary>
    public void UpdatePrice(long newPrice)
    {
        BasePrice = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}
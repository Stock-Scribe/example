namespace Nexon.FleaMarket.Domain.Entities;

public class Product
{
    public long ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int CategoryId { get; private set; }
    public string ImageUrl { get; private set; }
    public string DurationType { get; private set; } // PERMANENT / TEMPORARY
    public int? DurationDays { get; private set; }
    public int SellCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected Product() { }

    public Product(
        string productName,
        int categoryId,
        string durationType,
        int? durationDays = null,
        string imageUrl = null)
    {
        ProductName = productName;
        CategoryId = categoryId;
        DurationType = durationType;
        DurationDays = durationDays;
        ImageUrl = imageUrl;
        SellCount = 0;
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
    /// 판매 횟수 증가
    /// </summary>
    public void IncrementSellCount()
    {
        SellCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 상품명 변경
    /// </summary>
    public void UpdateName(string newName)
    {
        ProductName = newName;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 이미지 URL 변경
    /// </summary>
    public void UpdateImageUrl(string newImageUrl)
    {
        ImageUrl = newImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}
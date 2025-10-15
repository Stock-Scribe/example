namespace Nexon.FleaMarket.Domain.Entities;

public class Product
{
    // 식별자/속성
    public long ProductId { get; private set; }             // PK
    public string ProductName { get; private set; } = null!;
    public int CategoryId { get; private set; }           // FK (Category)
    public long BasePrice { get; private set; }           // 기본 가격 (price)
    public string DurationType { get; private set; } = null!; // "PERMANENT", "TEMPORARY"
    public int? DurationDays { get; private set; }          // 기간제일 경우 일수
    public string? ImageUrl { get; private set; }           // 상품 이미지 URL
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // EF Core용 기본 생성자
    private Product() { }

    // 팩토리 메서드
    public static Product Create(
        string productName,
        int categoryId,
        long basePrice,
        string durationType,
        int? durationDays = null,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.");
        if (categoryId <= 0)
            throw new ArgumentException("Invalid category id.");
        if (basePrice < 0)
            throw new ArgumentException("Base price must be non-negative.");
        if (string.IsNullOrWhiteSpace(durationType))
            throw new ArgumentException("DurationType is required.");

        // 임시 상품 생성
        return new Product
        {
            ProductName = productName.Trim(),
            CategoryId = categoryId,
            BasePrice = basePrice,
            DurationType = durationType.Trim().ToUpperInvariant(),
            DurationDays = durationType.Equals("TEMPORARY", StringComparison.OrdinalIgnoreCase)
                ? durationDays ?? throw new ArgumentException("DurationDays is required for TEMPORARY products.")
                : null,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // 상품명 수정
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Product name is required.");
        var trimmed = newName.Trim();
        if (string.Equals(ProductName, trimmed, StringComparison.Ordinal))
            return;
        ProductName = trimmed;
        UpdatedAt = DateTime.UtcNow;
    }

    // 가격 변경
    public void UpdatePrice(long newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative.");
        if (newPrice == BasePrice)
            return;
        BasePrice= newPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    // 기간 정보 변경
    public void UpdateDuration(string newType, int? newDays = null)
    {
        if (string.IsNullOrWhiteSpace(newType))
            throw new ArgumentException("DurationType is required.");

        var type = newType.Trim().ToUpperInvariant();

        if (type == "TEMPORARY")
        {
            if (newDays is null or <= 0)
                throw new ArgumentException("DurationDays must be positive for TEMPORARY products.");
            DurationDays = newDays;
        }
        else
        {
            DurationDays = null;
        }

        DurationType = type;
        UpdatedAt = DateTime.UtcNow;
    }
}
    

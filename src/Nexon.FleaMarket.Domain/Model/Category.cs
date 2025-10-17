namespace Nexon.FleaMarket.Domain.Entities;

public class Category
{
    public int CategoryId { get; private set; }                 // PK
    public string CategoryCode { get; private set; } = null!;   // 예: "weapon", "character"
    public string CategoryName { get; private set; } = null!;   // 예: "무기", "캐릭터"

    // 생성/수정 시각 (선택적)
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // EF Core용 기본 생성자
    private Category() { }

    // 팩토리 메서드
    public static Category Create(string categoryCode, string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryCode))
            throw new ArgumentException("Category code is required.");
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name is required.");

        return new Category
        {
            CategoryCode = categoryCode.Trim().ToLowerInvariant(),
            CategoryName = categoryName.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // 카테고리 이름 변경
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Category name is required.");
        var trimmed = newName.Trim();
        if (string.Equals(CategoryName, trimmed, StringComparison.Ordinal))
            return;
        CategoryName = trimmed;
        UpdatedAt = DateTime.UtcNow;
    }

    // 코드 변경 (거의 안 쓰이지만 대비)
    public void ChangeCode(string newCode)
    {
        if (string.IsNullOrWhiteSpace(newCode))
            throw new ArgumentException("Category code is required.");
        var trimmed = newCode.Trim().ToLowerInvariant();
        if (string.Equals(CategoryCode, trimmed, StringComparison.Ordinal))
            return;
        CategoryCode = trimmed;
        UpdatedAt = DateTime.UtcNow;
    }
}
namespace Nexon.FleaMarket.Domain.Entities;

public class User
{
    // 식별자/속성
    public long UserId { get; private set; }           // PK (EF Core가 관례로 인식)
    public string UserName { get; private set; } = null!;
    public long BalanceSp { get; private set; }        // Sudden Point
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // ORM/직렬화용 기본 생성자
    private User() { }

    // 생성 팩토리 (불변식 보장)
    public static User Create(string userName, long initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName is required.");
        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative.");
        return new User
        {
            UserName  = userName.Trim(),
            BalanceSp = initialBalance,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // 재화 충전
    public void Credit(long amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0.");
        checked
        {
            BalanceSp += amount;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    // 재화 차감
    public void Debit(long amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0.");
        if (BalanceSp < amount)
            throw new InvalidOperationException("Insufficient balance.");
        BalanceSp -= amount;
        UpdatedAt = DateTime.UtcNow;
    }

    // 이름 변경
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("UserName is required.");
        var trimmed = newName.Trim();
        if (string.Equals(UserName, trimmed, StringComparison.Ordinal))
            return; // 동일하면 변경 없음
        UserName  = trimmed;
        UpdatedAt = DateTime.UtcNow;
    }

    // 편의
    public bool CanAfford(long amount) => amount > 0 && BalanceSp >= amount;
}
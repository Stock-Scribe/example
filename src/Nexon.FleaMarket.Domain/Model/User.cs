namespace Nexon.FleaMarket.Domain.Entities;

/// <summary>
/// 사용자 엔티티
/// </summary>
public class User
{
    public long Id { get; private set; }
    public string Username { get; private set; }
    public long SpBalance { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected User() { }

    public User(string username)
    {
        Username = username;
        SpBalance = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// SP 잔액이 충분한지 확인
    /// </summary>
    public bool HasEnoughBalance(long requiredAmount)
    {
        return SpBalance >= requiredAmount;
    }

    /// <summary>
    /// SP 차감 (구매 시)
    /// </summary>
    public void DeductBalance(long amount)
    {
        if (!HasEnoughBalance(amount))
            throw new InvalidOperationException($"잔액 부족: 보유 {SpBalance} SP, 필요 {amount} SP");

        SpBalance -= amount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// SP 충전 (판매 수익 시)
    /// </summary>
    public void AddBalance(long amount)
    {
        SpBalance += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 이름 변경
    /// </summary>
    public void ChangeUsername(string newUsername)
    {
        Username = newUsername;
        UpdatedAt = DateTime.UtcNow;
    }
}
namespace Nexon.FleaMarket.Domain.Entities;

/// <summary>
/// 제한(밴) 엔티티
/// </summary>
public class RateLimitBan
{
    public long Id { get; private set; }
    public long? UserId { get; private set; }
    public string ClientIp { get; private set; }
    public string Action { get; private set; }
    public string Reason { get; private set; }
    public DateTime ReleaseAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected RateLimitBan() { }

    public RateLimitBan(
        long? userId,
        string clientIp,
        string action,
        string reason,
        TimeSpan banDuration)
    {
        UserId = userId;
        ClientIp = clientIp;
        Action = action;
        Reason = reason;
        CreatedAt = DateTime.UtcNow;
        ReleaseAt = DateTime.UtcNow.Add(banDuration);
    }

    /// <summary>
    /// 일정 시간 동안 밴 생성
    /// </summary>
    public static RateLimitBan CreateTemporaryBan(
        long? userId,
        string clientIp,
        string action,
        string reason,
        int durationMinutes)
    {
        return new RateLimitBan(userId, clientIp, action, reason, TimeSpan.FromMinutes(durationMinutes));
    }

    /// <summary>
    /// 밴이 아직 유효한지 확인
    /// </summary>
    public bool IsActive()
    {
        return DateTime.UtcNow < ReleaseAt;
    }

    /// <summary>
    /// 밴이 해제되었는지 확인
    /// </summary>
    public bool IsReleased()
    {
        return DateTime.UtcNow >= ReleaseAt;
    }

    /// <summary>
    /// 남은 밴 시간 (분)
    /// </summary>
    public int GetRemainingMinutes()
    {
        if (IsReleased()) return 0;
        return (int)Math.Ceiling((ReleaseAt - DateTime.UtcNow).TotalMinutes);
    }

    /// <summary>
    /// 밴 시간 연장
    /// </summary>
    public void ExtendBan(int additionalMinutes)
    {
        ReleaseAt = ReleaseAt.AddMinutes(additionalMinutes);
    }

    /// <summary>
    /// 즉시 밴 해제
    /// </summary>
    public void Release()
    {
        ReleaseAt = DateTime.UtcNow;
    }
}
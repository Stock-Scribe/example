namespace Nexon.FleaMarket.Domain.Entities;

/// <summary>
/// 요청 로그 엔티티
/// </summary>
public class RequestLog
{
    public long Id { get; private set; }
    public long? UserId { get; private set; }
    public string Action { get; private set; }
    public string ClientIp { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected RequestLog() { }

    public RequestLog(long? userId, string action, string clientIp)
    {
        UserId = userId;
        Action = action;
        ClientIp = clientIp;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 특정 액션의 로그인지 확인
    /// </summary>
    public bool IsAction(string actionName) => Action == actionName;

    /// <summary>
    /// 특정 시간 내의 로그인지 확인
    /// </summary>
    public bool IsWithinTimeRange(DateTime startTime, DateTime endTime)
    {
        return CreatedAt >= startTime && CreatedAt <= endTime;
    }
}
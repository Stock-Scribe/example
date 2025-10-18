namespace Nexon.FleaMarket.Application.Dto.response;

public class LoginResponse
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public long BalanceSP { get; set; }
}
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Application.Port.Input;

public interface IAuthUseCase
{
    /// <summary>
    /// 로그인
    /// </summary>
    Task<ApiResponse<LoginResponse>> LoginAsync(UserLoginRequest request);
}
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.Port.Input;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;

namespace Nexon.FleaMarket.Application.Service;

public class AuthService : IAuthUseCase
{
    private readonly IAuthPort _authPort;

    public AuthService(IAuthPort authPort)
    {
        _authPort = authPort;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(UserLoginRequest request)  // 👈 수정
    {
        // 입력값 검증
        if (string.IsNullOrWhiteSpace(request.UserEmail))
        {
            return ApiResponse<LoginResponse>.ErrorResponse(
                "이메일을 입력해주세요.",
                400
            );
        }

        if (string.IsNullOrWhiteSpace(request.UserPassword))
        {
            return ApiResponse<LoginResponse>.ErrorResponse(
                "비밀번호를 입력해주세요.",
                400
            );
        }

        // 이메일 형식 검증
        if (!request.UserEmail.Contains("@"))
        {
            return ApiResponse<LoginResponse>.ErrorResponse(
                "올바른 이메일 형식이 아닙니다.",
                400
            );
        }

        return await _authPort.LoginAsync(request);
    }
}
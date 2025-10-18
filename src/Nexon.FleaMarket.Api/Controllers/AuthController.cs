using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.Port.Input;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthUseCase _authUseCase;

    public AuthController
        (IAuthUseCase authUseCase)
    {
        _authUseCase = authUseCase;
    }

    /// <summary>
    /// 로그인
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 401)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 500)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] UserLoginRequest request)
    {
        var result = await _authUseCase.LoginAsync(request);
        return StatusCode(result.StatusCode, result);
    }
}
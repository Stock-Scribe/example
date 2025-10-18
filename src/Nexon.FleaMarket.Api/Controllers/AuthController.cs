using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Nexon.FleaMarket.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController
{
    private readonly ILoginUseCase _loginUseCase;

    public AuthController(ILoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    /// <summary>
    /// 로그인
    /// </summary>
    /// <param name="request">이메일, 비밀번호</param>
    /// <returns>유저 정보</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _loginUseCase.LoginAsync(request);
        
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }
}
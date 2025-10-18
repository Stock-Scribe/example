using Dapper;
using Microsoft.Data.SqlClient;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

/// <summary>
/// User Repository (로그인, 회원가입 등 DB 접근)
/// </summary>
public class AuthRepsitory : IAuthPort
{
    private readonly string _connectionString;

    public AuthRepsitory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// 로그인 (이메일/비밀번호 검증)
    /// </summary>
    public async Task<ApiResponse<LoginResponse>> LoginAsync(UserLoginRequest request)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT 
                    UserId,
                    UserName,
                    UserEmail,
                    BalanceSP
                FROM Users
                WHERE UserEmail = @UserEmail
                  AND UserPassword = @UserPassword";

            var user = await connection.QueryFirstOrDefaultAsync<LoginResponse>(sql, new
            {
                request.UserEmail,
                request.UserPassword
            });

            if (user == null)
            {
                return ApiResponse<LoginResponse>.ErrorResponse(
                    "이메일 또는 비밀번호가 일치하지 않습니다.",
                    401
                );
            }

            return ApiResponse<LoginResponse>.SuccessResponse(user, "로그인 성공");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponse>.ErrorResponse(
                $"로그인 처리 중 오류가 발생했습니다: {ex.Message}",
                500
            );
        }
    }
}
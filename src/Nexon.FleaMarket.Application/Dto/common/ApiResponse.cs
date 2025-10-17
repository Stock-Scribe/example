namespace Nexon.FleaMarket.Application.Dto.common;

/// <summary>
/// 통일된 API 응답 래퍼
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int StatusCode { get; set; }

    /// <summary>
    /// 성공 응답 생성
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = 200
        };
    }

    /// <summary>
    /// 실패 응답 생성
    /// </summary>
    public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            StatusCode = statusCode
        };
    }
}

/// <summary>
/// 데이터 없는 응답용
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public static new ApiResponse SuccessResponse(string message = "Success")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = null,
            StatusCode = 200
        };
    }

    public static new ApiResponse ErrorResponse(string message, int statusCode = 400)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null,
            StatusCode = statusCode
        };
    }
}
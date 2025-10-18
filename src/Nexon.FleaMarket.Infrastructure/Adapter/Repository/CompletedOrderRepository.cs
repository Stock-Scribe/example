using Dapper;
using Microsoft.Data.SqlClient;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public class CompletedOrderRepository: ICompletedOrderPort
{
    private readonly string _connectionString;

    public CompletedOrderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<ApiResponse<GetCompletedOrdersResponse>> GetCompletedOrdersAsync(
        GetCompletedOrdersRequest request)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var offset = (request.Page - 1) * request.PageSize;

            // WHERE 조건 생성
            string whereCondition = request.Type switch
            {
                "BUY" => "o.BuyerId = @UserId",
                "SELL" => "o.SellerId = @UserId",
                _ => "(o.BuyerId = @UserId OR o.SellerId = @UserId)"
            };

            // 전체 카운트
            var countSql = $@"
                    SELECT COUNT(*)
                    FROM Orders o
                    WHERE {whereCondition}
                      AND o.Status = 'COMPLETED'";

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { UserId = request.UserId });

            // 거래 완료 내역 조회
            var sql = $@"
                    SELECT 
                        o.OrderId,
                        o.ProductId,
                        p.ProductName,
                        p.ImageUrl,
                        o.Quantity,
                        o.TotalPrice,
                        CASE 
                            WHEN o.BuyerId = @UserId THEN 'BUY'
                            ELSE 'SELL'
                        END AS TradeType,
                        CASE 
                            WHEN o.BuyerId = @UserId THEN seller.UserName
                            ELSE buyer.UserName
                        END AS CounterPartyName,
                        o.CreatedAt
                    FROM Orders o
                    INNER JOIN Products p ON o.ProductId = p.ProductId
                    INNER JOIN Users buyer ON o.BuyerId = buyer.UserId
                    INNER JOIN Users seller ON o.SellerId = seller.UserId
                    WHERE {whereCondition}
                      AND o.Status = 'COMPLETED'
                    ORDER BY o.CreatedAt DESC
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY";

            var orders = await connection.QueryAsync<CompletedOrderDto>(sql, new
            {
                UserId = request.UserId,
                Offset = offset,
                PageSize = request.PageSize
            });

            var response = new GetCompletedOrdersResponse
            {
                Orders = orders.ToList(),
                TotalCount = totalCount,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                HasNextPage = (request.Page * request.PageSize) < totalCount
            };

            return ApiResponse<GetCompletedOrdersResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<GetCompletedOrdersResponse>.ErrorResponse(
                $"거래 완료 내역 조회 중 오류가 발생했습니다: {ex.Message}",
                500
            );
        }
    }
}
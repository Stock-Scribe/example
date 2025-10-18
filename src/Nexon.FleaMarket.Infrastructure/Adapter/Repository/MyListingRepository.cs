using Dapper;
using Microsoft.Data.SqlClient;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public class MyListingRepository : IMyListingsPort
{
    private readonly string _connectionString;
    
    public MyListingRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<ApiResponse<GetMyListingResponse>> GetMyListingsAsync(GetMyListingsRequest request)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var offset = (request.Page - 1) * request.PageSize;
            var listings = new List<MyListingDto>();
            var totalCount = 0;

            // Type에 따라 조회 분기
            switch (request.Type)
            {
                case "SELLING":
                    // 판매중만 (SellListings, Status=ACTIVE)
                    (listings, totalCount) = await GetSellListingsAsync(connection, request.UserId, "ACTIVE", offset, request.PageSize);
                    break;

                case "BUYING":
                    // 구매입찰중만 (BuyListings, Status=ACTIVE)
                    (listings, totalCount) = await GetBuyListingsAsync(connection, request.UserId, "ACTIVE", offset, request.PageSize);
                    break;

                case "CANCELLED":
                    // 취소된 것들 (둘 다, Status=CANCELLED)
                    var sellCancelled = await GetSellListingsAsync(connection, request.UserId, "CANCELLED", 0, 1000);
                    var buyCancelled = await GetBuyListingsAsync(connection, request.UserId, "CANCELLED", 0, 1000);
                    
                    listings = sellCancelled.Item1.Concat(buyCancelled.Item1)
                        .OrderByDescending(x => x.CreatedAt)
                        .Skip(offset)
                        .Take(request.PageSize)
                        .ToList();
                    
                    totalCount = sellCancelled.Item2 + buyCancelled.Item2;
                    break;

                case "ALL":
                default:
                    // 전체 (판매 + 구매)
                    var sellAll = await GetSellListingsAsync(connection, request.UserId, null, 0, 1000);
                    var buyAll = await GetBuyListingsAsync(connection, request.UserId, null, 0, 1000);
                    
                    listings = sellAll.Item1.Concat(buyAll.Item1)
                        .OrderByDescending(x => x.CreatedAt)
                        .Skip(offset)
                        .Take(request.PageSize)
                        .ToList();
                    
                    totalCount = sellAll.Item2 + buyAll.Item2;
                    break;
            }

            var response = new GetMyListingResponse
            {
                Listings = listings,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                HasNextPage = (request.Page * request.PageSize) < totalCount
            };

            return ApiResponse<GetMyListingResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<GetMyListingResponse>.ErrorResponse(
                $"내 리스팅 조회 중 오류가 발생했습니다: {ex.Message}",
                500
            );
        }
    }

    private async Task<(List<MyListingDto>, int)> GetSellListingsAsync(
        SqlConnection connection, long userId, string? status, int offset, int pageSize)
    {
        var whereClause = "sl.SellerId = @UserId";
        if (!string.IsNullOrEmpty(status))
        {
            whereClause += " AND sl.Status = @Status";
        }

        var countSql = $@"
            SELECT COUNT(*)
            FROM SellListings sl
            WHERE {whereClause}";

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { UserId = userId, Status = status });

        var sql = $@"
            SELECT 
                sl.ListingId,
                'SELL' AS ListingType,
                sl.ProductId,
                p.ProductName,
                p.ImageUrl,
                sl.ItemPrice,
                sl.Quantity,
                sl.Status,
                sl.CreatedAt
            FROM SellListings sl
            INNER JOIN Products p ON sl.ProductId = p.ProductId
            WHERE {whereClause}
            ORDER BY sl.CreatedAt DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        var listings = await connection.QueryAsync<MyListingDto>(sql, new
        {
            UserId = userId,
            Status = status,
            Offset = offset,
            PageSize = pageSize
        });

        return (listings.ToList(), totalCount);
    }

    private async Task<(List<MyListingDto>, int)> GetBuyListingsAsync(
        SqlConnection connection, long userId, string? status, int offset, int pageSize)
    {
        var whereClause = "bl.BuyerId = @UserId";
        if (!string.IsNullOrEmpty(status))
        {
            whereClause += " AND bl.Status = @Status";
        }

        var countSql = $@"
            SELECT COUNT(*)
            FROM BuyListings bl
            WHERE {whereClause}";

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { UserId = userId, Status = status });

        var sql = $@"
            SELECT 
                bl.BuyListingId AS ListingId,
                'BUY' AS ListingType,
                bl.ProductId,
                p.ProductName,
                p.ImageUrl,
                bl.ItemPrice,
                bl.Quantity,
                bl.Status,
                bl.CreatedAt
            FROM BuyListings bl
            INNER JOIN Products p ON bl.ProductId = p.ProductId
            WHERE {whereClause}
            ORDER BY bl.CreatedAt DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        var listings = await connection.QueryAsync<MyListingDto>(sql, new
        {
            UserId = userId,
            Status = status,
            Offset = offset,
            PageSize = pageSize
        });

        return (listings.ToList(), totalCount);
    }
}
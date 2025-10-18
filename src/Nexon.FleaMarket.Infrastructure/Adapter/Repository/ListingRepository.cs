using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

/// <summary>
/// Listing Repository (판매/구매 등록 DB 접근)
/// </summary>
public class ListingRepository : IListingPort
{
    private readonly string _connectionString;

    public ListingRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// 판매 등록 (SP 호출)
    /// </summary>
    public async Task<ApiResponse<CreateSellListingResponse>> CreateSellListingAsync(CreateSellListingRequest request)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", request.ProductId);
            parameters.Add("@SellerId", request.SellerId);
            parameters.Add("@ItemPrice", request.ItemPrice);
            parameters.Add("@Quantity", request.Quantity);
            parameters.Add("@IsMatched", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@OrderId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@ListingId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@ProcessedQuantity", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@RemainingQuantity", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@ResultCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@ResultMessage", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "usp_CreateSellListingWithMatch",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var resultCode = parameters.Get<int>("@ResultCode");
            var resultMessage = parameters.Get<string>("@ResultMessage");

            if (resultCode != 200)
            {
                return ApiResponse<CreateSellListingResponse>.ErrorResponse(resultMessage, resultCode);
            }

            var response = new CreateSellListingResponse
            {
                IsMatched = parameters.Get<bool>("@IsMatched"),
                OrderId = parameters.Get<long?>("@OrderId"),
                ListingId = parameters.Get<long?>("@ListingId"),
                ProcessedQuantity = parameters.Get<int>("@ProcessedQuantity"),
                RemainingQuantity = parameters.Get<int>("@RemainingQuantity")
            };

            return ApiResponse<CreateSellListingResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<CreateSellListingResponse>.ErrorResponse(
                $"판매 등록 처리 중 오류가 발생했습니다: {ex.Message}",
                500
            );
        }
    }

    /// <summary>
    /// 구매 입찰 (SP 호출)
    /// </summary>
    public async Task<ApiResponse<CreateBuyListingResponse>> CreateBuyListingAsync(CreateBuyListingRequest request)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", request.ProductId);
            parameters.Add("@BuyerId", request.BuyerId);
            parameters.Add("@ItemPrice", request.ItemPrice);
            parameters.Add("@Quantity", request.Quantity);
            parameters.Add("@IsMatched", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@OrderId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@BuyListingId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@ProcessedQuantity", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@RemainingQuantity", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@ResultCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@ResultMessage", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "usp_CreateBuyListingWithMatch",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var resultCode = parameters.Get<int>("@ResultCode");
            var resultMessage = parameters.Get<string>("@ResultMessage");

            if (resultCode != 200)
            {
                return ApiResponse<CreateBuyListingResponse>.ErrorResponse(resultMessage, resultCode);
            }

            var response = new CreateBuyListingResponse
            {
                ResultCode = resultCode,
                ResultMessage = resultMessage,
                IsMatched = parameters.Get<bool>("@IsMatched"),
                OrderId = parameters.Get<long?>("@OrderId"),
                BuyListingId = parameters.Get<long?>("@BuyListingId"),
                ProcessedQuantity = parameters.Get<int>("@ProcessedQuantity"),
                RemainingQuantity = parameters.Get<int>("@RemainingQuantity")
            };

            return ApiResponse<CreateBuyListingResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<CreateBuyListingResponse>.ErrorResponse(
                $"구매 입찰 처리 중 오류가 발생했습니다: {ex.Message}",
                500
            );
        }
    }

    /// <summary>
    /// 내 리스팅 조회
    /// </summary>
    public async Task<ApiResponse<GetMyListingResponse>> GetMyListingsAsync(GetMyListingsRequest request)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var offset = (request.Page - 1) * request.PageSize;
            var listings = new List<MyListingDto>();
            var totalCount = 0;

            switch (request.Type)
            {
                case "SELLING":
                    (listings, totalCount) = await GetSellListingsAsync(connection, request.UserId, "ACTIVE", offset, request.PageSize);
                    break;

                case "BUYING":
                    (listings, totalCount) = await GetBuyListingsAsync(connection, request.UserId, "ACTIVE", offset, request.PageSize);
                    break;

                case "CANCELLED":
                    var sellCancelled = await GetSellListingsAsync(connection, request.UserId, "CANCELLED", 0, 1000);
                    var buyCancelled = await GetBuyListingsAsync(connection, request.UserId, "CANCELLED", 0, 1000);
                    
                    var allCancelled = new List<MyListingDto>();
                    allCancelled.AddRange(sellCancelled.Item1);
                    allCancelled.AddRange(buyCancelled.Item1);
                    
                    listings = allCancelled
                        .OrderByDescending(x => x.CreatedAt)
                        .Skip(offset)
                        .Take(request.PageSize)
                        .ToList();
                    
                    totalCount = sellCancelled.Item2 + buyCancelled.Item2;
                    break;

                case "ALL":
                default:
                    var sellAll = await GetSellListingsAsync(connection, request.UserId, null, 0, 1000);
                    var buyAll = await GetBuyListingsAsync(connection, request.UserId, null, 0, 1000);
                    
                    var allListings = new List<MyListingDto>();
                    allListings.AddRange(sellAll.Item1);
                    allListings.AddRange(buyAll.Item1);
                    
                    listings = allListings
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

    // Private Helper Methods
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
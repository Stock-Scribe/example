using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public class SellListingRepository : ICreateSellListingPort
{
    private readonly string _connectionString;

    public SellListingRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

     /// <summary>
    /// 판매 등록 (자동 매칭 포함)
    /// </summary>
    public async Task<ApiResponse<CreateSellListingResponse>> CreateSellListingAsync(CreateSellListingRequest request)
    {
        using var connection = new SqlConnection(_connectionString);

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
        var isMatched = parameters.Get<bool>("@IsMatched");
        var orderId = parameters.Get<long?>("@OrderId");
        var listingId = parameters.Get<long?>("@ListingId");
        var processedQuantity = parameters.Get<int>("@ProcessedQuantity");
        var remainingQuantity = parameters.Get<int>("@RemainingQuantity");

        // 성공
        if (resultCode == 200)
        {
            return ApiResponse<CreateSellListingResponse>.SuccessResponse(
                new CreateSellListingResponse
                {
                    IsMatched = isMatched,
                    OrderId = orderId,
                    ListingId = listingId,
                    ProcessedQuantity = processedQuantity,
                    RemainingQuantity = remainingQuantity
                },
                resultMessage
            );
        }

        // 실패
        return ApiResponse<CreateSellListingResponse>.ErrorResponse(resultMessage, resultCode);
    
    }
}
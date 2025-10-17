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
    /// 판매 등록 (SP 호출)
    /// </summary>
    public async Task<ApiResponse<CreateSellListingResponse>> CreateSellListingAsync(CreateSellListingRequest request)
    {
        using var connection = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@ProductId", request.ProductId);
        parameters.Add("@SellerId", request.SellerId);
        parameters.Add("@ItemPrice", request.ItemPrice);
        parameters.Add("@Quantity", request.Quantity);
        parameters.Add("@ListingId", dbType: DbType.Int64, direction: ParameterDirection.Output);
        parameters.Add("@ResultCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ResultMessage", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            "usp_CreateSellListing",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var resultCode = parameters.Get<int>("@ResultCode");
        var resultMessage = parameters.Get<string>("@ResultMessage");
        var listingId = parameters.Get<long?>("@ListingId");

        // 성공
        if (resultCode == 200 && listingId.HasValue)
        {
            return ApiResponse<CreateSellListingResponse>.SuccessResponse(
                new CreateSellListingResponse { ListingId = listingId.Value },
                resultMessage
            );
        }

        // 실패
        return ApiResponse<CreateSellListingResponse>.ErrorResponse(resultMessage, resultCode);
    }
}
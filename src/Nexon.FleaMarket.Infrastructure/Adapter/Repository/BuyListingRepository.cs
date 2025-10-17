using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public class BuyListingRepository : ICreateBuyListingPort
{
    private readonly string _connectionString;

    public BuyListingRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// 구매 등록 (SP 호출)
    /// </summary>
    public async Task<ApiResponse<CreateBuyListingResponse>> CreateBuyListingAsync(
        CreateBuyListingRequest request,
        CancellationToken ct = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@ProductId", request.ProductId);
        parameters.Add("@BuyerId", request.BuyerId);
        parameters.Add("@BidPrice", request.BidPrice);
        parameters.Add("@Quantity", request.Quantity);
        parameters.Add("@BuyListingId", dbType: DbType.Int64, direction: ParameterDirection.Output);
        parameters.Add("@ResultCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ResultMessage", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            "usp_CreateBuyListing",
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 15
        );

        var resultCode = parameters.Get<int>("@ResultCode");
        var resultMessage = parameters.Get<string>("@ResultMessage");
        var buyListingId = parameters.Get<long?>("@BuyListingId");

        if (resultCode == 200 && buyListingId.HasValue)
        {
            return ApiResponse<CreateBuyListingResponse>.SuccessResponse(
                new CreateBuyListingResponse { BuyListingId = buyListingId.Value },
                resultMessage
            );
        }

        return ApiResponse<CreateBuyListingResponse>.ErrorResponse(resultMessage, resultCode);
    }
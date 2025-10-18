using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Infrastructure.Repository;


public class BuyListingRepository : ICreateBuyListingPort
{
    private readonly string _connectionString;

    public BuyListingRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

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
}
using System.Data;
using Microsoft.Data.SqlClient;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;


namespace Nexon.FleaMarket.Application.Service;
public class CreateBuyListingService : ICreateBuyListingUseCase
{
    private readonly ICreateBuyListingPort _createBuyListingPort;

    public CreateBuyListingService(ICreateBuyListingPort createBuyListingPort)
    {
        _createBuyListingPort = createBuyListingPort;
    }

    public async Task<ApiResponse<CreateBuyListingResponse>> CreateBuyListingAsync(CreateBuyListingRequest request)
    {
        // 1. 입력값 검증
        if (request.ProductId <= 0)
        {
            return ApiResponse<CreateBuyListingResponse>.ErrorResponse(
                "유효하지 않은 상품 ID입니다.",
                400
            );
        }

        if (request.BuyerId <= 0)
        {
            return ApiResponse<CreateBuyListingResponse>.ErrorResponse(
                "유효하지 않은 구매자 ID입니다.",
                400
            );
        }

        if (request.ItemPrice <= 0)
        {
            return ApiResponse<CreateBuyListingResponse>.ErrorResponse(
                "구매 가격은 0보다 커야 합니다.",
                400
            );
        }

        if (request.Quantity <= 0)
        {
            return ApiResponse<CreateBuyListingResponse>.ErrorResponse(
                "구매 수량은 0보다 커야 합니다.",
                400
            );
        }

        // 2. Repository 호출 (SP 실행 - 매칭 로직 포함)
        var result = await _createBuyListingPort.CreateBuyListingAsync(request);

        // 3. 결과에 따라 메시지 커스터마이징
        if (result.Success && result.Data != null)
        {
            if (result.Data.IsMatched && result.Data.RemainingQuantity > 0)
            {
                result.Message = $"일부 거래가 즉시 성사되었습니다! (거래: {result.Data.ProcessedQuantity}개, 입찰: {result.Data.RemainingQuantity}개)";
            }
            else if (result.Data.IsMatched)
            {
                result.Message = $"전체 수량이 즉시 거래 성사되었습니다! (수량: {result.Data.ProcessedQuantity}개)";
            }
            else
            {
                result.Message = $"구매 입찰이 완료되었습니다. (수량: {request.Quantity}개)";
            }
        }

        return result;
    }
}
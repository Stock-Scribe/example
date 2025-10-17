using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;

namespace Nexon.FleaMarket.Application.Service;

public class CreateSellListingService : ICreateSellListingUseCase
{
    private readonly ICreateSellListingPort _createSellListingPort;

    public CreateSellListingService(ICreateSellListingPort createSellListingPort)
    {
        _createSellListingPort = createSellListingPort;
    }

    public async Task<ApiResponse<CreateSellListingResponse>> CreateSellListingAsync(CreateSellListingRequest request)
    {
        // 1. 입력값 검증
        if (request.ProductId <= 0)
        {
            return ApiResponse<CreateSellListingResponse>.ErrorResponse(
                "유효하지 않은 상품 ID입니다.",
                400
            );
        }

        if (request.SellerId <= 0)
        {
            return ApiResponse<CreateSellListingResponse>.ErrorResponse(
                "유효하지 않은 판매자 ID입니다.",
                400
            );
        }

        if (request.ItemPrice <= 0)
        {
            return ApiResponse<CreateSellListingResponse>.ErrorResponse(
                "판매 가격은 0보다 커야 합니다.",
                400
            );
        }

        if (request.Quantity <= 0)
        {
            return ApiResponse<CreateSellListingResponse>.ErrorResponse(
                "판매 수량은 0보다 커야 합니다.",
                400
            );
        }

        // 2. Repository 호출 (SP 실행)
        var result = await _createSellListingPort.CreateSellListingAsync(request);

        // 3. 추가 비즈니스 로직 (나중에 추가 가능)
        // - 로깅
        // - 알림 전송
        // - 통계 업데이트

        return result;
    }
}
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

    public async Task<ApiResponse<CreateBuyListingResponse>> CreateBuyListingAsync(
        CreateBuyListingRequest request,
        CancellationToken ct = default)
    {
        // 1) 입력값 검증 (Sell과 동일 레벨)
        if (request.ProductId <= 0)
            return ApiResponse<CreateBuyListingResponse>.ErrorResponse("유효하지 않은 상품 ID입니다.", 400);

        if (request.BuyerId <= 0)
            return ApiResponse<CreateBuyListingResponse>.ErrorResponse("유효하지 않은 구매자 ID입니다.", 400);

        if (request.BidPrice <= 0)
            return ApiResponse<CreateBuyListingResponse>.ErrorResponse("구매 희망가는 0보다 커야 합니다.", 400);

        if (request.Quantity <= 0)
            return ApiResponse<CreateBuyListingResponse>.ErrorResponse("구매 수량은 0보다 커야 합니다.", 400);

        // 2) Repository 호출 (SP 실행)
        var result = await _createBuyListingPort.CreateBuyListingAsync(request, ct);

        // 3) 후처리 훅(로깅/알림 등) 필요시 여기에
        
        return result;
    }
}
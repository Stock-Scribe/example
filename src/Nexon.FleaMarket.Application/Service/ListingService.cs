using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.Port;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;

namespace Nexon.FleaMarket.Application.Service;

/// <summary>
/// Listing Service (판매/구매 등록 비즈니스 로직)
/// </summary>
public class ListingService : IListingUseCase
{
    private readonly IListingPort _listingPort;

    public ListingService(IListingPort listingPort)
    {
        _listingPort = listingPort;
    }

    /// <summary>
    /// 판매 등록
    /// </summary>
    public async Task<ApiResponse<CreateSellListingResponse>> CreateSellListingAsync(CreateSellListingRequest request)
    {
        // 입력값 검증
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

        // Repository 호출
        var result = await _listingPort.CreateSellListingAsync(request);

        // 결과 메시지 커스터마이징
        if (result.Success && result.Data != null)
        {
            if (result.Data.IsMatched)
            {
                result.Message = $"거래가 즉시 성사되었습니다! (수량: {result.Data.ProcessedQuantity})";
            }
            else
            {
                result.Message = $"판매 등록이 완료되었습니다. (수량: {result.Data.ProcessedQuantity})";
            }
        }

        return result;
    }

    /// <summary>
    /// 구매 입찰 등록
    /// </summary>
    public async Task<ApiResponse<CreateBuyListingResponse>> CreateBuyListingAsync(CreateBuyListingRequest request)
    {
        // 입력값 검증
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

        // Repository 호출
        var result = await _listingPort.CreateBuyListingAsync(request);

        // 결과 메시지 커스터마이징
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

    /// <summary>
    /// 내 리스팅 조회
    /// </summary>
    public async Task<ApiResponse<GetMyListingResponse>> GetMyListingsAsync(GetMyListingsRequest request)
    {
        // 입력값 검증
        if (request.UserId <= 0)
        {
            return ApiResponse<GetMyListingResponse>.ErrorResponse(
                "유효하지 않은 유저 ID입니다.",
                400
            );
        }

        if (request.PageSize <= 0 || request.PageSize > 100)
        {
            request.PageSize = 20;
        }

        if (request.Page <= 0)
        {
            request.Page = 1;
        }

        if (request.Type != "ALL" && request.Type != "SELLING" && 
            request.Type != "BUYING" && request.Type != "CANCELLED")
        {
            request.Type = "ALL";
        }

        return await _listingPort.GetMyListingsAsync(request);
    }
}
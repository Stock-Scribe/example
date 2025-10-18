using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public interface IListingPort
{
    /// <summary>
    /// 판매 등록 (SP 호출)
    /// </summary>
    Task<ApiResponse<CreateSellListingResponse>> CreateSellListingAsync(CreateSellListingRequest request);

    /// <summary>
    /// 구매 입찰 (SP 호출)
    /// </summary>
    Task<ApiResponse<CreateBuyListingResponse>> CreateBuyListingAsync(CreateBuyListingRequest request);

    /// <summary>
    /// 내 리스팅 조회
    /// </summary>
    Task<ApiResponse<GetMyListingResponse>> GetMyListingsAsync(GetMyListingsRequest request);
    
}
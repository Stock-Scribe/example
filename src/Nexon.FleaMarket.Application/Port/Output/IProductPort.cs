using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public interface IProductPort
{
    /// <summary>
    /// 상품 검색 (전체 조회 + 필터링 + 페이징)
    /// </summary>
    Task<ApiResponse<PagedData<ProductResponse>>> SearchProductsAsync(ProductSearchRequest request);
}
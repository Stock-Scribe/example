using Nexon.FleaMarket.Application.Dtos.request;
using Nexon.FleaMarket.Application.Dtos.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public interface IProductRepository
{
    /// <summary>
    /// 상품 검색 (전체 조회 + 필터링)
    /// </summary>
    Task<PagedResult<ProductResponse>> SearchProductsAsync(ProductSearchRequest request);
}
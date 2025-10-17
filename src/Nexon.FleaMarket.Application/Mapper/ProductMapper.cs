using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Domain.VO;

namespace Nexon.FleaMarket.Application.Mapper;

public static class ProductMapper
{
    /// ProductSearchRequest DTO → ProductSearchQuery Domain VO 변환
    public static ProductSearchQuery ToQuery(this ProductSearchRequest request)
    {
        return new ProductSearchQuery
        {
            Keyword = request.SearchKeyword,
            CategoryId = request.CategoryId,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy
        };
    }
    
}
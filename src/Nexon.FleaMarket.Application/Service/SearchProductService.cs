using Nexon.FleaMarket.Application.UseCase;

namespace Nexon.FleaMarket.Application.Service;

public class SearchProductsService : ISearchProductUseCase
{
    private readonly IProductRepository _productRepository;

    public SearchProductsService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductResponse>> ExecuteAsync(ProductSearchRequest request)
    {
        // 입력 검증 (나중에 추가 가능)
        // if (request.Page < 1) throw new ArgumentException("Page must be >= 1");
        
        // Repository 호출
        var result = await _productRepository.SearchProductsAsync(request);
        
        // 추가 비즈니스 로직 (나중에 추가 가능)
        // - 로깅
        // - 조회수 증가
        // - 추천 알고리즘
        
        return result;
    }
}
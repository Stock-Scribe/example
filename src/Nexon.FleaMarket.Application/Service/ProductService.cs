using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;

namespace Nexon.FleaMarket.Application.Service;

public class ProductsService : IProductUseCase
{
    private readonly IProductPort _productPort;

    public ProductsService(IProductPort productPort)
    {
        _productPort = productPort;
    }

    public async Task<ApiResponse<PagedData<ProductResponse>>> SearchProducts(ProductSearchRequest request)
    {
        // Repository 호출
        var result = await _productPort.SearchProductsAsync(request);
        return result;
    }
}
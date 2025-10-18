using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Application.UseCase;

public interface IProductUseCase
{
    Task<ApiResponse<PagedData<ProductResponse>>> SearchProducts(ProductSearchRequest request);
}
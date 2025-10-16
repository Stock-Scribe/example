using Nexon.FleaMarket.Application.Dtos.request;
using Nexon.FleaMarket.Application.Dtos.response;

namespace Nexon.FleaMarket.Application.UseCase;

public interface ISearchProductUseCase
{
    Task<PagedResult<ProductResponse>> ExecuteAsync(ProductSearchRequest request);
}
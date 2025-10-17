using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Application.UseCase;

public interface ICreateSellListingUseCase
{
    Task<ApiResponse<CreateSellListingResponse>> CreateSellListingAsync(CreateSellListingRequest request);
}
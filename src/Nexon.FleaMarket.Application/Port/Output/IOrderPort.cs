using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public interface IOrderPort
{
    Task<ApiResponse<GetCompletedOrdersResponse>> GetCompletedOrdersAsync(GetCompletedOrdersRequest request);
}
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;

namespace Nexon.FleaMarket.Application.Service;

public class OrderService: IOrderUseCase
{
    private readonly IOrderPort _ordersPort;
    public OrderService(IOrderPort ordersPort)
    {
        _ordersPort = ordersPort;
    }
    public async Task<ApiResponse<GetCompletedOrdersResponse>> GetCompletedOrdersAsync(GetCompletedOrdersRequest request)
    {
        if (request.UserId <= 0)
        {
            return ApiResponse<GetCompletedOrdersResponse>.ErrorResponse(
                "유효하지 않은 유저 ID입니다.",
                400
            );
        }

        if (request.PageSize <= 0 || request.PageSize > 100)
        {
            request.PageSize = 20;
        }

        if (request.Page <= 0)
        {
            request.Page = 1;
        }

        if (request.Type != "ALL" && request.Type != "BUY" && request.Type != "SELL")
        {
            request.Type = "ALL";
        }

        return await _ordersPort.GetCompletedOrdersAsync(request);
    }
}
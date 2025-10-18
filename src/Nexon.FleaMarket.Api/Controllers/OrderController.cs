using Microsoft.AspNetCore.Mvc;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.UseCase;

namespace Nexon.FleaMarket.Api.Controllers;


[ApiController]
[Route("api/orders")]
public class OrderController: ControllerBase
{
    
    private readonly IOrderUseCase _ordersUseCase;

    public OrderController(IOrderUseCase ordersUseCase)
    {
        _ordersUseCase = ordersUseCase;
    }

    /// <summary>
    /// 거래 완료 내역 조회
    /// </summary>
    /// <param name="request">조회 조건</param>
    /// <returns>거래 완료 내역</returns>
    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedOrders([FromQuery] GetCompletedOrdersRequest request)
    {
        var result = await _ordersUseCase.GetCompletedOrdersAsync(request);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }
        
        return Ok(result);
    }
}
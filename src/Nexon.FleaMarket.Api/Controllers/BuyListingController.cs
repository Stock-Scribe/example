using Microsoft.AspNetCore.Mvc;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.UseCase;

namespace Nexon.FleaMarket.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class BuyController : ControllerBase
{
    private readonly ICreateBuyListingUseCase _createBuyListingUseCase;

    public BuyController(ICreateBuyListingUseCase createBuyListingUseCase)
    {
        _createBuyListingUseCase = createBuyListingUseCase;
    }

    /// <summary>구매 시도</summary>
    [HttpPost("attempt")]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 404)]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 409)]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 500)]
    public async Task<ActionResult<ApiResponse<CreateBuyListingResponse>>> Attempt(
        [FromBody] CreateBuyListingRequest request,
        CancellationToken ct)
    {
        var result = await _createBuyListingUseCase.CreateBuyListingAsync(request, ct);
        return StatusCode(result.StatusCode, result);
    }
}
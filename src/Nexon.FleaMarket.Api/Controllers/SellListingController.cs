using Microsoft.AspNetCore.Mvc;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.UseCase;

namespace Nexon.FleaMarket.Api.Controllers;

[ApiController]
[Route("api/sell-listing")]
public class SellListingController: ControllerBase
{   
    private readonly ICreateSellListingUseCase _createSellListingUseCase;

    public SellListingController(ICreateSellListingUseCase createSellListingUseCase)
    {
        _createSellListingUseCase = createSellListingUseCase;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateSellListingResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CreateSellListingResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<CreateSellListingResponse>), 404)]
    [ProducesResponseType(typeof(ApiResponse<CreateSellListingResponse>), 500)]
    public async Task<ActionResult<ApiResponse<CreateSellListingResponse>>> CreateSellListing(
        [FromBody] CreateSellListingRequest request)
    {
        var result = await _createSellListingUseCase.CreateSellListingAsync(request);
        return StatusCode(result.StatusCode, result);
    }
    
}
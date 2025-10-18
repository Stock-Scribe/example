using Microsoft.AspNetCore.Mvc;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.Port;
using Nexon.FleaMarket.Application.UseCase;

namespace Nexon.FleaMarket.Api.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingController : ControllerBase
{
    private readonly IListingUseCase _listingUseCase;

    public ListingController(IListingUseCase listingUseCase)
    {
        _listingUseCase = listingUseCase;
    }

    /// <summary>
    /// 판매 등록
    /// </summary>
    [HttpPost("sell")]
    [ProducesResponseType(typeof(ApiResponse<CreateSellListingResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CreateSellListingResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<CreateSellListingResponse>), 404)]
    [ProducesResponseType(typeof(ApiResponse<CreateSellListingResponse>), 500)]
    public async Task<ActionResult<ApiResponse<CreateSellListingResponse>>> CreateSellListing(
        [FromBody] CreateSellListingRequest request)
    {
        var result = await _listingUseCase.CreateSellListingAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// 구매 입찰
    /// </summary>
    [HttpPost("buy")]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 404)]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 409)]
    [ProducesResponseType(typeof(ApiResponse<CreateBuyListingResponse>), 500)]
    public async Task<ActionResult<ApiResponse<CreateBuyListingResponse>>> CreateBuyListing(
        [FromBody] CreateBuyListingRequest request)
    {
        var result = await _listingUseCase.CreateBuyListingAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// 내 리스팅 조회 (판매/구매 등록 내역)
    /// </summary>
    [HttpGet("myPage")]
    [ProducesResponseType(typeof(ApiResponse<GetMyListingResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<GetMyListingResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<GetMyListingResponse>), 500)]
    public async Task<ActionResult<ApiResponse<GetMyListingResponse>>> GetMyListings(
        [FromQuery] GetMyListingsRequest request)
    {
        var result = await _listingUseCase.GetMyListingsAsync(request);
        
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }
}
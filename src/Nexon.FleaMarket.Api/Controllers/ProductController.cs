using Microsoft.AspNetCore.Mvc;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // -> api/product 주소로 자동 매핑
public class ProductController : ControllerBase
{
    private readonly IProductUseCase _productUseCase;
    public ProductController(IProductUseCase productUseCase)
    {
        _productUseCase = productUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedData<ProductResponse>>>> SearchProducts(
        [FromQuery] ProductSearchRequest request)
    {
        var result = await _productUseCase.SearchProducts(request);
        return StatusCode(result.StatusCode, result);
    }
}
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
    // 1. 주입받을 서비스(Use Case)를 위한 private readonly 필드 선언
    private readonly ISearchProductUseCase _searchProductUseCase;

    // 2. 생성자(Constructor)를 통해 서비스(Use Case)를 주입받음
    //    - .NET의 DI 컨테이너가 Program.cs에 등록된 SearchProductService 인스턴스를
    //      자동으로 찾아서 여기에 넣어줍니다.
    public ProductController(ISearchProductUseCase searchProductUseCase)
    {
        _searchProductUseCase = searchProductUseCase;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedData<ProductResponse>>>> SearchProducts(
        [FromQuery] ProductSearchRequest request) 
    {
        var result = await _searchProductUseCase.SearchProducts(request);  
        return StatusCode(result.StatusCode, result);
    }
}
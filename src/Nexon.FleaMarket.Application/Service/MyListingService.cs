using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;

namespace Nexon.FleaMarket.Application.Service;

public class MyListingService: IGetMyListingUseCase
{
    private readonly IMyListingsPort _myListingsPort;

    public MyListingService(IMyListingsPort myListingsPort)
    {
        _myListingsPort = myListingsPort;
    }

    public async Task<ApiResponse<GetMyListingResponse>> GetMyListingsAsync(GetMyListingsRequest request)
    {
        if (request.UserId <= 0)
        {
            return ApiResponse<GetMyListingResponse>.ErrorResponse(
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

        if (request.Type != "ALL" && request.Type != "SELLING" && 
            request.Type != "BUYING" && request.Type != "CANCELLED")
        {
            request.Type = "ALL";
        }

        return await _myListingsPort.GetMyListingsAsync(request);    
    }
}
namespace Nexon.FleaMarket.Application.Dto.response;

public class GetMyListingResponse
{
    public List<MyListingDto> Listings { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
}
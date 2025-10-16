namespace Nexon.FleaMarket.Application.Dtos.response;

/// <summary>
/// 상품 응답
/// </summary>
public class ProductResponse
{
    public long ProductId { get; set; }
    public int ExternalItemNo { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public long BasePrice { get; set; }
    public string DurationType { get; set; }
    public int? DurationDays { get; set; }
    public string ImageUrl { get; set; }
    
    // 거래소 정보
    public long? LowestPrice { get; set; }      // 최저 판매가
    public int? AvailableQuantity { get; set; } // 판매 가능 수량
}

/// <summary>
/// 페이지네이션 응답
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNext => Page < TotalPages;
}
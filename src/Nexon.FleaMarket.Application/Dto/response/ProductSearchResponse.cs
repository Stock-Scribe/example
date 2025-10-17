namespace Nexon.FleaMarket.Application.Dto.response;

/// <summary>
/// 상품 응답
/// </summary>
public class ProductResponse
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string DurationType { get; set; } = string.Empty;
    public int? DurationDays { get; set; }
    public int SellCount { get; set; }
    
    // 거래소 정보
    public long? LowestPrice { get; set; }      // 최저 판매가 (현재 판매 중인 리스팅 중 최저가)
    public int? AvailableQuantity { get; set; } // 판매 가능 수량 (ACTIVE 리스팅의 총 수량)
}
/// <summary>
/// 페이지네이션 응답 데이터
/// </summary>
public class PagedData<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNext => Page < TotalPages;
}
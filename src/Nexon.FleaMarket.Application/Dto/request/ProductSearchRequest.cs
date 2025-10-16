namespace Nexon.FleaMarket.Application.Dtos.request;

public class ProductSearchRequest
{
    /// <summary>
    /// 검색 키워드 (상품명)
    /// </summary>
    public string? SearchKeyword { get; set; }
    
    /// <summary>
    /// 카테고리 ID 필터
    /// </summary>
    public int? CategoryId { get; set; }
    
    /// <summary>
    /// 최소 가격
    /// </summary>
    public long? MinPrice { get; set; }
    
    /// <summary>
    /// 최대 가격
    /// </summary>
    public long? MaxPrice { get; set; }
    
    /// <summary>
    /// 정렬 기준 (price, latest)
    /// </summary>
    public string SortBy { get; set; } = "price";
    
    /// <summary>
    /// 페이지 번호 (1부터 시작)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// 페이지 크기 (기본 20개)
    /// </summary>
    public int PageSize { get; set; } = 20;
}
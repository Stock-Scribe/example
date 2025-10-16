using Nexon.FleaMarket.Application.Dtos.request;
using Nexon.FleaMarket.Application.Dtos.response;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Nexon.FleaMarket.Infrastructure.Repository;
public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<ProductResponse>> SearchProductsAsync(ProductSearchRequest request)
    {
        using var connection = new SqlConnection(_connectionString);

        // 동적 WHERE 조건 생성
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        // 검색 키워드
        if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
        {
            conditions.Add("p.ProductName LIKE @SearchKeyword");
            parameters.Add("SearchKeyword", $"%{request.SearchKeyword}%");
        }

        // 카테고리 필터
        if (request.CategoryId.HasValue)
        {
            conditions.Add("p.CategoryId = @CategoryId");
            parameters.Add("CategoryId", request.CategoryId.Value);
        }

        // 최소 가격
        if (request.MinPrice.HasValue)
        {
            conditions.Add("p.BasePriceSP >= @MinPrice");
            parameters.Add("MinPrice", request.MinPrice.Value);
        }

        // 최대 가격
        if (request.MaxPrice.HasValue)
        {
            conditions.Add("p.BasePriceSP <= @MaxPrice");
            parameters.Add("MaxPrice", request.MaxPrice.Value);
        }

        var whereClause = conditions.Any()
            ? "WHERE " + string.Join(" AND ", conditions)
            : "";

        // 정렬
        var orderBy = request.SortBy?.ToLower() switch
        {
            "latest" => "ORDER BY p.CreatedAt DESC",
            "price" => "ORDER BY p.BasePriceSP ASC",
            _ => "ORDER BY p.BasePriceSP ASC"
        };

        // 페이지네이션
        var offset = (request.Page - 1) * request.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", request.PageSize);

        // 전체 개수 조회
        var countQuery = $@"
            SELECT COUNT(*)
            FROM Products p
            {whereClause}";

        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

        // 상품 목록 조회 (최저가 포함)
        var query = $@"
            SELECT 
                p.ProductId,
                p.ExternalItemNo,
                p.ProductName,
                c.CategoryName,
                p.BasePriceSP AS BasePrice,
                p.DurationType,
                p.DurationDays,
                p.ImageUrl,
                (
                    SELECT MIN(UnitPriceSP)
                    FROM SellListings
                    WHERE ProductId = p.ProductId 
                      AND Status = 'ACTIVE'
                      AND Quantity > 0
                ) AS LowestPrice,
                (
                    SELECT SUM(Quantity)
                    FROM SellListings
                    WHERE ProductId = p.ProductId 
                      AND Status = 'ACTIVE'
                ) AS AvailableQuantity
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.CategoryId
            {whereClause}
            {orderBy}
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        var products = await connection.QueryAsync<ProductResponse>(query, parameters);

        return new PagedResult<ProductResponse>
        {
            Items = products.ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
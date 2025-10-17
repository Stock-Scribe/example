using Dapper;
using Microsoft.Data.SqlClient;
using Nexon.FleaMarket.Application.Dto.common;
using Nexon.FleaMarket.Application.Dto.request;
using Nexon.FleaMarket.Application.Dto.response;

namespace Nexon.FleaMarket.Infrastructure.Repository;

public class ProductRepository : IProductPort
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// 상품 목록 조회 (검색, 필터링, 정렬, 페이징)
    /// </summary>
    public async Task<ApiResponse<PagedData<ProductResponse>>> SearchProductsAsync(ProductSearchRequest request)
    {
        using var connection = new SqlConnection(_connectionString);

        // 동적 WHERE 조건 생성
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        // 1. 검색 키워드 (상품명)
        if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
        {
            conditions.Add("p.ProductName LIKE @SearchKeyword");
            parameters.Add("SearchKeyword", $"%{request.SearchKeyword}%");
        }

        // 2. 카테고리 필터
        if (request.CategoryId.HasValue)
        {
            conditions.Add("p.CategoryId = @CategoryId");
            parameters.Add("CategoryId", request.CategoryId.Value);
        }

        // 3. 가격 필터 (최저가 기준)
        if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
        {
            conditions.Add(@"
                p.ProductId IN (
                    SELECT ProductId 
                    FROM SellListings 
                    WHERE Status = 'ACTIVE' 
                      AND Quantity > 0
                      {0}
                )");

            var priceConditions = new List<string>();
            if (request.MinPrice.HasValue)
            {
                priceConditions.Add("UnitPriceSP >= @MinPrice");
                parameters.Add("MinPrice", request.MinPrice.Value);
            }
            if (request.MaxPrice.HasValue)
            {
                priceConditions.Add("UnitPriceSP <= @MaxPrice");
                parameters.Add("MaxPrice", request.MaxPrice.Value);
            }

            var lastCondition = conditions[conditions.Count - 1];
            conditions[conditions.Count - 1] = string.Format(
                lastCondition, 
                priceConditions.Any() ? "AND " + string.Join(" AND ", priceConditions) : ""
            );
        }

        var whereClause = conditions.Any()
            ? "WHERE " + string.Join(" AND ", conditions)
            : "";

        // 4. 정렬
        var orderBy = request.SortBy?.ToLower() switch
        {
            "price" => @"ORDER BY (
                SELECT MIN(UnitPriceSP) 
                FROM SellListings 
                WHERE ProductId = p.ProductId 
                  AND Status = 'ACTIVE' 
                  AND Quantity > 0
            ) ASC",
            "latest" => "ORDER BY p.CreatedAt DESC",
            _ => "ORDER BY p.CreatedAt DESC"
        };

        // 5. 페이지네이션 (20개 단위)
        var offset = (request.Page - 1) * request.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", request.PageSize);

        // 전체 개수 조회
        var countQuery = $@"
            SELECT COUNT(*)
            FROM Products p
            {whereClause}";

        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

        // 상품 목록 조회
        var query = $@"
            SELECT 
                p.ProductId,
                p.ProductName,
                p.CategoryId,
                c.CategoryName,
                p.ImageUrl,
                p.DurationType,
                p.DurationDays,
                p.SellCount,
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

        var pagedData = new PagedData<ProductResponse>
        {
            Items = products.ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedData<ProductResponse>>.SuccessResponse(
            pagedData,
            "상품 목록 조회 성공"
        );
    }
}
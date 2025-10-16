- =============================================
-- SP_InstantBuy: 즉시 구매 처리
-- =============================================
CREATE OR ALTER PROCEDURE SP_InstantBuy
    -- 입력 파라미터
    @BuyerId BIGINT,              -- 구매자 ID
    @ListingId BIGINT,            -- 판매 등록 ID
    @Quantity INT,                -- 구매 수량
    @IdempotencyKey VARCHAR(64),  -- 중복 방지 키

    -- 출력 파라미터
    @OrderId BIGINT OUTPUT,           -- 생성된 주문 ID
    @ErrorMessage NVARCHAR(500) OUTPUT  -- 에러 메시지
    AS
BEGIN
    SET NOCOUNT ON;
    
    -- 변수 선언
    DECLARE @ProductId BIGINT;
    DECLARE @SellerId BIGINT;
    DECLARE @UnitPrice BIGINT;
    DECLARE @TotalPrice BIGINT;
    DECLARE @BuyerBalance BIGINT;
    DECLARE @AvailableQuantity INT;

BEGIN TRY
        -- 트랜잭션 시작
BEGIN TRANSACTION;
        
        -- ========================================
        -- 1단계: 중복 요청 확인 (멱등성)
        -- ========================================
        IF EXISTS (SELECT 1 FROM Orders WHERE IdempotencyKey = @IdempotencyKey)
BEGIN
            -- 이미 처리된 요청이면 기존 주문 반환
SELECT @OrderId = OrderId FROM Orders WHERE IdempotencyKey = @IdempotencyKey;
SET @ErrorMessage = NULL;
COMMIT TRANSACTION;
RETURN 0; -- 성공
END
        
        -- ========================================
        -- 2단계: 판매 정보 조회 (Lock 걸기)
        -- ========================================
SELECT
    @ProductId = ProductId,
    @SellerId = SellerId,
    @UnitPrice = UnitPriceSP,
    @AvailableQuantity = Quantity
FROM SellListings WITH (UPDLOCK, ROWLOCK)  -- 다른 사람이 못 건드리게 Lock!
WHERE ListingId = @ListingId
  AND Status = 'ACTIVE';

-- 판매 등록이 없으면 에러
IF @ProductId IS NULL
BEGIN
            SET @ErrorMessage = N'판매 등록을 찾을 수 없거나 활성 상태가 아닙니다.';
ROLLBACK TRANSACTION;
RETURN -1;
END
        
        -- ========================================
        -- 3단계: 재고 확인
        -- ========================================
        IF @AvailableQuantity < @Quantity
BEGIN
            SET @ErrorMessage = N'재고가 부족합니다. (보유: ' + CAST(@AvailableQuantity AS NVARCHAR) + 
                              N'개, 요청: ' + CAST(@Quantity AS NVARCHAR) + N'개)';
ROLLBACK TRANSACTION;
RETURN -2;
END
        
        -- ========================================
        -- 4단계: 자기 자신 구매 방지
        -- ========================================
        IF @BuyerId = @SellerId
BEGIN
            SET @ErrorMessage = N'본인의 판매 등록은 구매할 수 없습니다.';
ROLLBACK TRANSACTION;
RETURN -3;
END
        
        -- ========================================
        -- 5단계: 총 가격 계산
        -- ========================================
        SET @TotalPrice = @UnitPrice * @Quantity;
        
        -- ========================================
        -- 6단계: 구매자 잔액 조회 (Lock 걸기)
        -- ========================================
SELECT @BuyerBalance = BalanceSP
FROM Users WITH (UPDLOCK, ROWLOCK)
WHERE UserId = @BuyerId;

-- 구매자가 없으면 에러
IF @BuyerBalance IS NULL
BEGIN
            SET @ErrorMessage = N'구매자를 찾을 수 없습니다.';
ROLLBACK TRANSACTION;
RETURN -4;
END
        
        -- ========================================
        -- 7단계: 잔액 확인
        -- ========================================
        IF @BuyerBalance < @TotalPrice
BEGIN
            SET @ErrorMessage = N'잔액이 부족합니다. (보유: ' + CAST(@BuyerBalance AS NVARCHAR) + 
                              N' SP, 필요: ' + CAST(@TotalPrice AS NVARCHAR) + N' SP)';
ROLLBACK TRANSACTION;
RETURN -5;
END
        
        -- ========================================
        -- 8단계: 구매자 돈 차감
        -- ========================================
UPDATE Users
SET BalanceSP = BalanceSP - @TotalPrice,
    UpdatedAt = SYSUTCDATETIME()
WHERE UserId = @BuyerId;

-- ========================================
-- 9단계: 판매자 돈 증가
-- ========================================
UPDATE Users
SET BalanceSP = BalanceSP + @TotalPrice,
    UpdatedAt = SYSUTCDATETIME()
WHERE UserId = @SellerId;

-- ========================================
-- 10단계: 판매 재고 감소
-- ========================================
UPDATE SellListings
SET Quantity = Quantity - @Quantity,
    Status = CASE
                 WHEN (Quantity - @Quantity) = 0 THEN 'SOLD_OUT'
                 ELSE Status
        END,
    UpdatedAt = SYSUTCDATETIME()
WHERE ListingId = @ListingId;

-- ========================================
-- 11단계: 주문 생성
-- ========================================
INSERT INTO Orders (
    ListingId,
    BuyListingId,
    BuyerId,
    SellerId,
    ProductId,
    Quantity,
    TotalPriceSP,
    Status,
    IdempotencyKey,
    CreatedAt,
    UpdatedAt
)
VALUES (
           @ListingId,
           NULL,  -- 즉시 구매는 BuyListingId 없음
           @BuyerId,
           @SellerId,
           @ProductId,
           @Quantity,
           @TotalPrice,
           'PAID',
           @IdempotencyKey,
           SYSUTCDATETIME(),
           SYSUTCDATETIME()
       );

SET @OrderId = SCOPE_IDENTITY(); -- 방금 생성된 주문 ID
        
        -- ========================================
        -- 12단계: 구매자 인벤토리에 아이템 추가
        -- ========================================
        -- 만료일 계산
        DECLARE @ExpiresAt DATETIME2;
        DECLARE @DurationType VARCHAR(20);
        DECLARE @DurationDays INT;

SELECT @DurationType = DurationType, @DurationDays = DurationDays
FROM Products
WHERE ProductId = @ProductId;

IF @DurationType = 'TEMPORARY' AND @DurationDays IS NOT NULL
            SET @ExpiresAt = DATEADD(DAY, @DurationDays, SYSUTCDATETIME());
ELSE
            SET @ExpiresAt = NULL; -- 영구 아이템
        
        -- 인벤토리 추가
INSERT INTO UserInventory (
    UserId,
    ProductId,
    SourceOrderId,
    Quantity,
    ExpireAt,
    AcquiredAt
)
VALUES (
           @BuyerId,
           @ProductId,
           @OrderId,
           @Quantity,
           @ExpiresAt,
           SYSUTCDATETIME()
       );

-- ========================================
-- 13단계: 주문 완료 처리
-- ========================================
UPDATE Orders
SET Status = 'FULFILLED',
    UpdatedAt = SYSUTCDATETIME()
WHERE OrderId = @OrderId;

-- 트랜잭션 커밋 (모든 변경사항 확정!)
COMMIT TRANSACTION;

SET @ErrorMessage = NULL;
RETURN 0; -- 성공!

END TRY
BEGIN CATCH
        -- 에러 발생 시 모든 변경사항 취소
IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SET @ErrorMessage = ERROR_MESSAGE();
RETURN -999; -- 시스템 오류
END CATCH
END
GO
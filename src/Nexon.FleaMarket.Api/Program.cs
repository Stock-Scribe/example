// Program.cs (최종 수정본)

// 1. 아키텍처 구조에 맞는 정확한 using 문을 사용합니다.
using Nexon.FleaMarket.Application.Service;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =================================================================
// ★★★ 의존성 주입(DI) 최종 설정 ★★★

// 2. appsettings.json에서 연결 문자열을 안전하게 가져옵니다.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 3. Repository(Adapter)를 등록할 때, 생성자에 필요한 connectionString을 직접 넘겨줍니다.
builder.Services.AddScoped<IProductPort>(provider => 
    new ProductRepository(connectionString));

// 4. Service(UseCase)를 등록합니다.
builder.Services.AddScoped<ISearchProductUseCase, SearchProductsService>();

// SellListing (판매 등록)
builder.Services.AddScoped<ICreateSellListingPort>(provider => 
    new SellListingRepository(connectionString));
builder.Services.AddScoped<ICreateSellListingUseCase, CreateSellListingService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// UseAuthorization()는 .NET 6+ 템플릿에 기본 포함될 수 있으나, 현재 인증 기능이 없으므로 주석 처리하거나 제거해도 괜찮습니다.
// app.UseAuthorization(); 

app.MapControllers();

app.Run();
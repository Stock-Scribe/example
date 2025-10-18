using Nexon.FleaMarket.Application.Port;
using Nexon.FleaMarket.Application.Port.Input;
using Nexon.FleaMarket.Application.Service;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ========== CORS 설정 추가 ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Next.js 주소
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Product (상품 검색)
builder.Services.AddScoped<IProductPort>(provider => 
    new ProductRepository(connectionString));
builder.Services.AddScoped<IProductUseCase, ProductsService>();

// Listing (판매/구매 등록)
builder.Services.AddScoped<IListingPort>(provider => 
    new ListingRepository(connectionString));
builder.Services.AddScoped<IListingUseCase, ListingService>();

// Order (거래 완료 내역)
builder.Services.AddScoped<IOrderPort>(provider => 
    new OrderRepository(connectionString));
builder.Services.AddScoped<IOrderUseCase, OrderService>();

// User (로그인, 회원가입)
builder.Services.AddScoped<IAuthPort>(provider => 
    new AuthRepsitory(connectionString));
builder.Services.AddScoped<IAuthUseCase, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();
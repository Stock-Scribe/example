using Nexon.FleaMarket.Application.Service;
using Nexon.FleaMarket.Application.UseCase;
using Nexon.FleaMarket.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 연결 문자열
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Product
builder.Services.AddScoped<IProductPort>(provider => 
    new ProductRepository(connectionString));
builder.Services.AddScoped<ISearchProductUseCase, SearchProductsService>();

// SellListing
builder.Services.AddScoped<ICreateSellListingPort>(provider => 
    new SellListingRepository(connectionString));
builder.Services.AddScoped<ICreateSellListingUseCase, CreateSellListingService>();

// BuyListing
builder.Services.AddScoped<ICreateBuyListingPort>(provider => 
    new BuyListingRepository(connectionString));
builder.Services.AddScoped<ICreateBuyListingUseCase, CreateBuyListingService>();



builder.Services.AddScoped<ICompletedOrderPort>(provider => 
    new CompletedOrderRepository(connectionString));
builder.Services.AddScoped<IGetCompletedOrderUseCase,CompletedOrderService>();

// 내 리스팅 조회
builder.Services.AddScoped<IMyListingsPort>(provider => 
    new MyListingRepository(connectionString));
builder.Services.AddScoped<IGetMyListingUseCase, MyListingService>();
    


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
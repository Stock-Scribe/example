using Nexon.FleaMarket.Infrastructure.Repositories;
using Nexon.FleaMarket.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ 임시로 직접 입력 (테스트용)
var connectionString = "Server=localhost,1433;Database=FleaMarket;User Id=sa;Password=Paul2858;TrustServerCertificate=True;";

Console.WriteLine($"ConnectionString: {connectionString}");

builder.Services.AddScoped<IUserRepository>(provider => 
    new UserRepository(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
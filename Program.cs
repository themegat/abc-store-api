
using ABCStoreAPI.Configuration;
using ABCStoreAPI.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiConfig>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpClient();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ABCStoreAPI.Service.ProductImportService>();
builder.Services.AddScoped<ABCStoreAPI.Service.ExchangeRateImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Running in Development environment");
    MigrateDatabase();
    app.MapOpenApi();
}

SeedData();
LoadExchangeRates();
LoadProducts();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

void MigrateDatabase()
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

void LoadProducts()
{
    using var scope = app.Services.CreateScope();
    var productService = scope.ServiceProvider.GetRequiredService<ABCStoreAPI.Service.ProductImportService>();
    productService.RunProductsImportAsync().GetAwaiter().GetResult();
}

void LoadExchangeRates()
{
    using var scope = app.Services.CreateScope();
    var exchangeRateService = scope.ServiceProvider.GetRequiredService<ABCStoreAPI.Service.ExchangeRateImportService>();
    exchangeRateService.RunExchangeRateImportAsync().GetAwaiter().GetResult();
}

void SeedData()
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();   
    var seeders = new List<DataSeeder>
    {
        new ABCStoreAPI.Database.Seeder.FromCodeSeeder(dbContext, logger)
    };

    foreach (var seeder in seeders)
    {
        seeder.SeedAsync().GetAwaiter().GetResult();
    }
}

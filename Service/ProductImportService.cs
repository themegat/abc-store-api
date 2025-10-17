using ABCStoreAPI.Configurations;
using ABCStoreAPI.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ABCStoreAPI.Service;

class DummyJsonProduct
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

class DummyJsonProducts
{
    public List<DummyJsonProduct>? Products { get; set; }
}

public class ProductImportService
{
    private readonly ILogger<ProductImportService> _logger;
    private const string SysUser = "system";
    private readonly HttpClient _httpClient;
    private readonly string _productApiUrl;
    private readonly AppDbContext _dbContext;

    public ProductImportService(HttpClient httpClient, IOptions<ApiConfig> apiConfig,
    AppDbContext dbContext, ILogger<ProductImportService> logger)
    {
        _httpClient = httpClient;
        _productApiUrl = apiConfig.Value.ProductApiUrl;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ImportProductsAsync()
    {
        var response = await _httpClient.GetAsync(_productApiUrl);
        response.EnsureSuccessStatusCode();
        string data = await response.Content.ReadAsStringAsync();

        DummyJsonProducts importProducts = JsonConvert.DeserializeObject<DummyJsonProducts>(data)!;

        if (importProducts != null && importProducts.Products != null)
        {
            foreach (var product in importProducts.Products)
            {
                var newProduct = new Product()
                {
                    Name = product.Title,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.Stock
                };
                newProduct.CreatedAt = DateTime.UtcNow;
                newProduct.UpdatedAt = DateTime.UtcNow;
                newProduct.CreatedBy = SysUser;
                newProduct.UpdatedBy = SysUser;
                _dbContext.Products.Add(newProduct);
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Imported {Count} products.", importProducts.Products.Count);
        }
    }
}

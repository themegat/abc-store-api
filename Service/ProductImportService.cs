using ABCStoreAPI.Configurations;
using ABCStoreAPI.Model;
using Microsoft.EntityFrameworkCore;
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

    private async Task<bool> IsExistingProduct(Product product) =>
        await _dbContext.Products.FirstOrDefaultAsync(p => p.Name == product.Name) != null;

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
        int newCount = 0;
        int duplicateCount = 0;

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

                if (!await IsExistingProduct(newProduct))
                {
                    _dbContext.Products.Add(newProduct);
                    newCount++;
                }else
                {
                    duplicateCount++;
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Added {Count} products. Skipped {Skipped} duplicate products.", newCount, duplicateCount);
        }
    }
}

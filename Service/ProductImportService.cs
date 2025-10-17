using ABCStoreAPI.Configuration;
using ABCStoreAPI.Database;
using ABCStoreAPI.Database.Model;
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
    public string Category { get; set; } = string.Empty;

    public string Thumbnail { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new List<string>();
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

    private async Task<bool> IsExistingProduct(string name) =>
        await _dbContext.Product.FirstOrDefaultAsync(p => p.Name == name) != null;
    private async Task<bool> IsExistingCategory(string name) =>
       await _dbContext.ProductCategory.FirstOrDefaultAsync(p => p.Name == name) != null;

    public ProductImportService(HttpClient httpClient, IOptions<ApiConfig> apiConfig,
    AppDbContext dbContext, ILogger<ProductImportService> logger)
    {
        _httpClient = httpClient;
        _productApiUrl = apiConfig.Value.ProductApiUrl;
        _dbContext = dbContext;
        _logger = logger;
    }

    private async Task ImportProductCategories(DummyJsonProducts importProducts)
    {
        int newCount = 0;
        int duplicateCount = 0;

        if (importProducts != null && importProducts.Products != null)
        {
            HashSet<string> categoryNames = new HashSet<string>();
            foreach (var product in importProducts.Products)
            {
                categoryNames.Add(product.Category);
            }

            foreach (var categoryName in categoryNames)
            {
                if (!await IsExistingCategory(categoryName))
                {
                    var newCategory = new ProductCategory()
                    {
                        Name = categoryName
                    };
                    newCategory.CreatedAt = DateTime.UtcNow;
                    newCategory.UpdatedAt = DateTime.UtcNow;
                    newCategory.CreatedBy = SysUser;
                    newCategory.UpdatedBy = SysUser;

                    _dbContext.ProductCategory.Add(newCategory);
                    newCount++;
                }
                else
                {
                    duplicateCount++;
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Added {Count} product categories. Skipped {Skipped} duplicate categories.", newCount, duplicateCount);
        }
    }

    private async Task PersistProductImages(string name, List<string> imageUrls)
    {
        var product = await _dbContext.Product.FirstOrDefaultAsync(p => p.Name == name);

        if (product != null)
        {
            foreach (var imageUrl in imageUrls)
            {

                var productImage = new ProductImage()
                {
                    Url = imageUrl,
                    ProductId = product.Id
                };
                productImage.CreatedAt = DateTime.UtcNow;
                productImage.UpdatedAt = DateTime.UtcNow;
                productImage.CreatedBy = SysUser;
                productImage.UpdatedBy = SysUser;

                _dbContext.ProductImage.Add(productImage);
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task ImportProductsAsync(DummyJsonProducts importProducts)
    {
        int newCount = 0;
        int duplicateCount = 0;
        List<Tuple<string, List<string>>> productsToImages = new List<Tuple<String, List<string>>>();

        if (importProducts != null && importProducts.Products != null)
        {
            foreach (var product in importProducts.Products)
            {
                var newProduct = new Product()
                {
                    Name = product.Title,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.Stock,
                    ThumbnailUrl = product.Thumbnail
                };
                newProduct.CreatedAt = DateTime.UtcNow;
                newProduct.UpdatedAt = DateTime.UtcNow;
                newProduct.CreatedBy = SysUser;
                newProduct.UpdatedBy = SysUser;
                var category = await _dbContext.ProductCategory.FirstOrDefaultAsync(c => c.Name == product.Category);
                if (category != null)
                {
                    newProduct.ProductCategoryId = category.Id;
                }

                if (!await IsExistingProduct(newProduct.Name))
                {
                    _dbContext.Product.Add(newProduct);
                    productsToImages.Add(new Tuple<string, List<string>>(newProduct.Name, product.Images));
                    newCount++;
                }
                else
                {
                    duplicateCount++;
                }
            }

            await _dbContext.SaveChangesAsync();
            foreach (var pi in productsToImages)
            {
                await PersistProductImages(pi.Item1, pi.Item2);
            }

            _logger.LogInformation("Added {Count} products. Skipped {Skipped} duplicate products.", newCount, duplicateCount);
        }
    }

    public async Task RunProductsImportAsync()
    {
        var response = await _httpClient.GetAsync(_productApiUrl);
        response.EnsureSuccessStatusCode();
        string data = await response.Content.ReadAsStringAsync();

        DummyJsonProducts importProducts = JsonConvert.DeserializeObject<DummyJsonProducts>(data)!;

        await ImportProductCategories(importProducts);
        await ImportProductsAsync(importProducts);
    }
}

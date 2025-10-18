using ABCStoreAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace ABCStoreAPI.Service;

public class ProductService
{
    private readonly IUnitOfWork _uow;

    public ProductService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<DataTransfer.ProductCategoryDto>> GetAllProductCategoriesAsync()
    {
        var categories = await _uow.ProductCategories.GetAll().ToListAsync();
        return categories.Select(DataTransfer.ProductCategoryDto.toDto).ToList();
    }

    public async Task<List<DataTransfer.ProductDto>> GetAllProductsAsync(string currencyCode = "USD")
    {
        var products = await _uow.Products.GetAll()
        .Include(p => p.ProductCategory)
        .Include(p => p.ProductImages)
        .ToListAsync();
        return products.Select(DataTransfer.ProductDto.toDto)
        .Select(p => { p.Price = ConvertPriceAsync(p.Price, currencyCode).Result; return p; })
        .ToList();
    }

    public async Task<List<DataTransfer.ProductDto>> GetProductsByCategoryAsync(int categoryId, string currencyCode = "USD")
    {
        var products = await _uow.Products.GetAllByCategory(categoryId)
        .Include(p => p.ProductCategory)
        .Include(p => p.ProductImages)
        .ToListAsync();
        return products.Select(DataTransfer.ProductDto.toDto)
        .Select(p => { p.Price = ConvertPriceAsync(p.Price, currencyCode).Result; return p; })
        .ToList();
    }

    public async Task<List<DataTransfer.ProductDto>> SearchProductsAsync(string searchTerm, string currencyCode = "USD")
    {
        var products = await _uow.Products.SearchBy(searchTerm)
        .Include(p => p.ProductCategory)
        .Include(p => p.ProductImages)
        .ToListAsync();

        return products.Select(DataTransfer.ProductDto.toDto)
        .Select(p => { p.Price = ConvertPriceAsync(p.Price, currencyCode).Result; return p; })
        .ToList();
    }

    private async Task<decimal> ConvertPriceAsync(decimal price, string targetCurrencyCode)
    {
        if (targetCurrencyCode == "USD")
        {
            return price;
        }

        var exchangeRate = await _uow.ExchangeRates
        .GetByCurrency(targetCurrencyCode)
        .FirstOrDefaultAsync();

        if (exchangeRate == null)
        {
            throw new Exception($"Exchange rate for currency code '{targetCurrencyCode}' not found.");
        }

        return price * exchangeRate.Rate;
    }
}

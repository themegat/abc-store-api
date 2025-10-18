using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABCStoreAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly Service.ProductService _productService;

        public ProductController(Service.ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("category/{categoryId}/{currencyCode?}")]
        public async Task<IActionResult> GetProductByCategory(int categoryId, string currencyCode = "USD")
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId, currencyCode);
            return Ok(products);
        }

        [HttpGet]
        [Route("all/{currencyCode?}")]
        public async Task<IActionResult> GetAllProducts(string currencyCode = "USD")
        {
            var products = await _productService.GetAllProductsAsync(currencyCode);
            return Ok(products);
        }

        [HttpGet]
        [Route("search/{searchTerm}/{currencyCode?}")]
        public async Task<IActionResult> SearchProducts(string searchTerm, string currencyCode = "USD")
        {
            var products = await _productService.SearchProductsAsync(searchTerm, currencyCode);
            return Ok(products);
        }

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetAllProductCategories()
        {
            var categories = await _productService.GetAllProductCategoriesAsync();
            return Ok(categories);
        }
    }
}

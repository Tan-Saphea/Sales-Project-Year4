using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SALESPURCHASEDB.Models;
using SALESPURCHASEDB.Repositories;

namespace SALESPURCHASEDB.Services;

public class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(ProductRepository? productRepository = null)
    {
        _productRepository = productRepository ?? new ProductRepository();
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await _productRepository.GetProductsAsync();
    }

    public List<Product> GetProducts() => GetProductsAsync().GetAwaiter().GetResult();

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        if (productId <= 0)
            throw new ArgumentException("Invalid Product ID.", nameof(productId));

        return await _productRepository.GetProductByIdAsync(productId);
    }

    public Product? GetProductById(int productId) => GetProductByIdAsync(productId).GetAwaiter().GetResult();

    public async Task<int> InsertProductAsync(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");

        if (string.IsNullOrWhiteSpace(product.ProductName))
            throw new ArgumentException("Product name cannot be empty.", nameof(product.ProductName));

        if (product.CategoryID <= 0)
            throw new ArgumentException("A valid Category ID must be selected.", nameof(product.CategoryID));

        if (product.UnitPriceIn < 0)
            throw new ArgumentException("UnitPriceIn cannot be negative.", nameof(product.UnitPriceIn));

        if (product.UnitPriceOut < 0)
            throw new ArgumentException("UnitPriceOut cannot be negative.", nameof(product.UnitPriceOut));

        if (product.Qty < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(product.Qty));

        return await _productRepository.InsertProductAsync(product);
    }

    public int InsertProduct(Product product) => InsertProductAsync(product).GetAwaiter().GetResult();

    public async Task<bool> UpdateProductAsync(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");

        if (product.ProductID <= 0)
            throw new ArgumentException("Invalid Product ID for update.", nameof(product.ProductID));

        if (string.IsNullOrWhiteSpace(product.ProductName))
            throw new ArgumentException("Product name cannot be empty.", nameof(product.ProductName));

        if (product.CategoryID <= 0)
            throw new ArgumentException("A valid Category ID must be selected.", nameof(product.CategoryID));

        if (product.UnitPriceIn < 0)
            throw new ArgumentException("UnitPriceIn cannot be negative.", nameof(product.UnitPriceIn));

        if (product.UnitPriceOut < 0)
            throw new ArgumentException("UnitPriceOut cannot be negative.", nameof(product.UnitPriceOut));

        if (product.Qty < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(product.Qty));

        return await _productRepository.UpdateProductAsync(product);
    }

    public bool UpdateProduct(Product product) => UpdateProductAsync(product).GetAwaiter().GetResult();

    public async Task<bool> DeleteProductAsync(int productId)
    {
        if (productId <= 0)
            throw new ArgumentException("Invalid Product ID for delete.", nameof(productId));

        return await _productRepository.DeleteProductAsync(productId);
    }

    public bool DeleteProduct(int productId) => DeleteProductAsync(productId).GetAwaiter().GetResult();
}
